using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Services;

public class EcheanceProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EcheanceProcessorService> _logger;

    // ── Update this value when TMM changes ──────────────────────────
    private const decimal CurrentTmm = 7.0m;

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
            var now = DateTime.Now;
            var delay = DateTime.Today.AddDays(1).AddHours(2) - now;
            //var delay = TimeSpan.FromMinutes(1); // Testing

            _logger.LogInformation("Next processing at: {NextRun}", now.Add(delay));
            await Task.Delay(delay, stoppingToken);

            try
            {
                await ProcessTodayEcheancesAsync();
                await ProcessTodayDeblocagesAsync(); // ← add this
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during background processing.");
            }
        }
    }

    public async Task ProcessTodayDeblocagesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DigiBankContext>();

        var today = DateOnly.FromDateTime(DateTime.Today);
        _logger.LogInformation("Processing déblocages for date: {Today}", today);

        var creditsToDebloquer = await context.Credits
            .Include(c => c.IdCompteNavigation)
            .Where(c =>
                c.DateDeblocage == today &&
                c.IdCompteNavigation != null)
            .ToListAsync();

        if (!creditsToDebloquer.Any())
        {
            _logger.LogInformation("No déblocages due today.");
            return;
        }

        _logger.LogInformation("Found {Count} crédit(s) to débloquer.", creditsToDebloquer.Count);

        foreach (var credit in creditsToDebloquer)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var compte = credit.IdCompteNavigation!;

                compte.Solde += credit.Montant ?? 0;
                compte.SoldeDisponible += credit.Montant ?? 0;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Déblocage processed for crédit {CreditId}: added {Montant} to compte {CompteId}.",
                    credit.Id, credit.Montant, compte.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,
                    "Failed to process déblocage for crédit {CreditId}. Transaction rolled back.",
                    credit.Id);
            }
        }
    }

    public async Task ProcessTodayEcheancesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DigiBankContext>();

        var today = DateOnly.FromDateTime(DateTime.Today);
        _logger.LogInformation("Processing écheances for date: {Today}", today);

        var dueEcheances = await context.Echeances
            .Include(e => e.IdCreditNavigation)
                .ThenInclude(c => c!.IdCompteNavigation)
            .Where(e =>
                e.DatePaiement == today &&
                e.Etat == 1 &&
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

                decimal totalDue = echeance.EcheanceMontant ?? 0;

                // ── 1. Deduct from account ───────────────────────────────────
                compte.Solde -= totalDue;

                compte.SoldeDisponible -= totalDue;

                // ── 2. Update TMM and interest rate on credit ────────────────
                decimal oldTmm = credit.Tmm ?? 0;
                credit.Tmm = CurrentTmm;
                credit.TauxInteret = CurrentTmm + (credit.MargeBanque ?? 0);

                // ── 3. Mark current écheance as paid ─────────────────────────
                echeance.Etat = 2;

                // ── 4. Find and update next écheance ─────────────────────────
                int nextNumero = echeance.NumeroEcheance + 1;
                bool hasNext = nextNumero <= (credit.NbEcheance ?? 0)
                               && (echeance.CapitalRestant ?? 0) > 0;

                if (hasNext)
                {
                    var existingNext = await context.Echeances.FirstOrDefaultAsync(e =>
                        e.IdCredit == credit.Id &&
                        e.NumeroEcheance == nextNumero);

                    if (existingNext != null)
                    {
                        decimal oldAmount = existingNext.EcheanceMontant ?? 0;

                        // Recalculate interest with new TMM
                        decimal newTauxAnnuel = (credit.Tmm ?? 0) + (credit.MargeBanque ?? 0);
                        decimal r = newTauxAnnuel / 12m / 100m;
                        decimal newInteret = Math.Round((existingNext.CapitalRestant ?? 0) * r, 3);

                        // Update next écheance values
                        existingNext.Interet = newInteret;
                        existingNext.EcheanceMontant = Math.Round(
                            (existingNext.CapitalRembourse ?? 0) + newInteret + (credit.FraisAdditionel ?? 0), 3);
                        existingNext.Etat = 1;

                        credit.MontantRembourse= (credit.MontantRembourse ?? 0) + (echeance.EcheanceMontant ?? 0);

                        // Adjust MontantTotalARembourser with the difference
                        decimal difference = (existingNext.EcheanceMontant ?? 0) - oldAmount;
                        credit.MontantTotalARembourser = (credit.MontantTotalARembourser ?? 0) + difference;

                        // Update CurrentEcheance on the credit to point to next
                        credit.CurrentEcheance = existingNext.Id;


                        _logger.LogInformation(
                            "Updated écheance #{Num} for credit {CreditId}. " +
                            "New interest: {Interet}, New total: {Total}. " +
                            "TMM updated from {OldTmm} to {NewTmm}. " +
                            "MontantTotal adjusted by {Diff}.",
                            existingNext.NumeroEcheance, credit.Id,
                            existingNext.Interet, existingNext.EcheanceMontant,
                            oldTmm, CurrentTmm, difference);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Next écheance #{Num} not found for credit {CreditId}.",
                            nextNumero, credit.Id);
                    }
                }
                else
                {
                    _logger.LogInformation(
                        "Credit {CreditId} fully repaid. No next écheance to update.", credit.Id);
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
}