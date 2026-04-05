using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Services;

public class EcheanceProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EcheanceProcessorService> _logger;

    public EcheanceProcessorService(
        IServiceScopeFactory scopeFactory,
        ILogger<EcheanceProcessorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EcheanceProcessorService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Calculate delay until next run at 02:00 AM
            var now = DateTime.Now;
            var nextRun = DateTime.Today.AddDays(1).AddHours(2);
            var delay = nextRun - now;

            _logger.LogInformation("Next écheance processing scheduled at: {NextRun}", nextRun);
            await Task.Delay(delay, stoppingToken);

            try
            {
                await ProcessTodayEcheancesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing écheances.");
            }
        }
    }

    private async Task ProcessTodayEcheancesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DigiBankContext>();

        var today = DateOnly.FromDateTime(DateTime.Today);
        _logger.LogInformation("Processing écheances for date: {Today}", today);

        // Get all due écheances for today with statut = 1 (active/due)
        var dueEcheances = await context.Echeances
            .Include(e => e.IdCreditNavigation)
                .ThenInclude(c => c!.IdCompteNavigation)
            .Where(e =>
                e.DateEcheance == today &&
                e.Statut == 1 &&
                e.IdCreditNavigation != null &&
                e.IdCreditNavigation.IdCompteNavigation != null)
            .ToListAsync();

        if (!dueEcheances.Any())
        {
            _logger.LogInformation("No écheances due today.");
            return;
        }

        _logger.LogInformation("Found {Count} écheance(s) to process.", dueEcheances.Count);

        foreach (var echeance in dueEcheances)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var credit = echeance.IdCreditNavigation!;
                var compte = credit.IdCompteNavigation!;

                decimal totalDue = echeance.TotalEcheance ?? 0;

                // ── 1. Deduct from account balance ──────────────────────────
                if (compte.Solde < totalDue)
                {
                    _logger.LogWarning(
                        "Insufficient balance for compte {CompteId}. " +
                        "Required: {Required}, Available: {Available}. Skipping.",
                        compte.Id, totalDue, compte.Solde);
                    await transaction.RollbackAsync();
                    continue;
                }

                compte.Solde -= totalDue;
                compte.SoldeDisponible -= totalDue;

                // ── 2. Mark écheance as paid ─────────────────────────────────
                echeance.Statut = 2; // 2 = Paid
                echeance.DatePaiement = today;
                echeance.InteretRembourse =  echeance.Interet;
                echeance.InteretRestant = 0;

                // ── 3. Calculate and insert next écheance ────────────────────
                var nextEcheance = CalculateNextEcheance(echeance, credit);
                if (nextEcheance != null)
                {
                    context.Echeances.Add(nextEcheance);
                    _logger.LogInformation(
                        "Next écheance #{Num} created for credit {CreditId}, due {Date}.",
                        nextEcheance.NumeroEcheance, credit.Id, nextEcheance.DateEcheance);
                }
                else
                {
                    _logger.LogInformation(
                        "Credit {CreditId} fully repaid. No next écheance created.", credit.Id);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Écheance {EcheanceId} processed. Deducted {Amount} from compte {CompteId}.",
                    echeance.Id, totalDue, compte.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,
                    "Failed to process écheance {EcheanceId}. Transaction rolled back.",
                    echeance.Id);
            }
        }
    }

    /// <summary>
    /// Calculates the next écheance using the Tunisian banking formula
    /// (constant monthly instalment — French amortisation method).
    ///
    /// Formulas:
    ///   Monthly rate  : r  = taux_annuel / 12 / 100
    ///   Interest       : I  = capital_restant × r
    ///   Amortissement  : A  = mensualité_constante - I
    ///   Capital restant: CR = capital_restant_précédent - A
    ///   Ass Vie        : AV = capital_restant × (taux_ass_vie  / 12 / 100)
    ///   Ass Incapacité : AI = capital_restant × (taux_ass_inc  / 12 / 100)
    ///   Total          : T  = A + I + AV + AI
    /// </summary>
    private Echeance? CalculateNextEcheance(Echeance current, Credit credit)
    {
        // Remaining capital after this payment
        decimal capitalRestant = (current.CapitalRestant ?? 0) - (current.Amortissement ?? 0);

        // Credit is fully repaid
        if (capitalRestant <= 0)
            return null;

        int nextNumero = current.NumeroEcheance + 1;
        int totalEcheances = credit.NbEcheance ?? 0;

        // All scheduled instalments already processed
        if (nextNumero > totalEcheances)
            return null;

        decimal tauxAnnuel = credit.Taux ?? 0;          // e.g. 9.50 for 9.50 %
        decimal tauxAssVie = credit.TauxAssVie ?? 0;
        decimal tauxAssInc = credit.TauxAssInc ?? 0;

        decimal r = tauxAnnuel / 12m / 100m;             // monthly interest rate

        // Constant monthly principal+interest payment (annuity formula)
        // M = P × r / (1 − (1+r)^−n)
        int remainingPeriods = totalEcheances - current.NumeroEcheance;
        decimal mensualite;

        if (r == 0)
        {
            mensualite = capitalRestant / remainingPeriods;
        }
        else
        {
            decimal factor = (decimal)Math.Pow((double)(1 + r), -remainingPeriods);
            mensualite = capitalRestant * r / (1 - factor);
        }

        decimal interet = Math.Round(capitalRestant * r, 2);
        decimal amortissement = Math.Round(mensualite - interet, 2);
        decimal newCapital = Math.Round(capitalRestant - amortissement, 2);

        // Insurance premiums (applied on remaining capital)
        decimal assVie = Math.Round(capitalRestant * (tauxAssVie / 12m / 100m), 2);
        decimal assInc = Math.Round(capitalRestant * (tauxAssInc / 12m / 100m), 2);

        decimal total = Math.Round(amortissement + interet + assVie + assInc, 2);

        // Next due date: add 1 month (same day of month)
        var nextDate = current.DateEcheance.AddMonths(1);

        return new Echeance
        {
            IdCredit = current.IdCredit,
            NumeroEcheance = nextNumero,
            DateEcheance = nextDate,
            CapitalRembourse = amortissement,
            CapitalRestant = newCapital,
            Interet = interet,
            Amortissement = amortissement,
            AssVie = assVie,
            AssInc = assInc,
            IntAdd = 0,
            TotalEcheance = total,
            Statut = 1,   // active / due
            InteretRembourse = 0,
            InteretRestant = interet
        };
    }
}