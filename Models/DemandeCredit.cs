using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class DemandeCredit
{
    public int Id { get; set; }

    public string? Reference { get; set; }

    public DateOnly? DateEnvoi { get; set; }

    public int? IdClient { get; set; }

    public int? IdCompte { get; set; }

    public string TypeCredit { get; set; } = null!;

    public string? NatureCredit { get; set; }

    public string? Objet { get; set; }

    public decimal? Montant { get; set; }

    public int? DureeMois { get; set; }

    public short? Periodicite { get; set; }

    public int? DureeGrace { get; set; }

    public int? Etat { get; set; }

    public string? CinPathFront { get; set; }

    public string? CinPathBack { get; set; }

    public string? IndicateurResidencePath { get; set; }

    public int? IdCredit { get; set; }

    public string? FicheDePaiePath { get; set; }

    public string? AttestationDeTravailPath { get; set; }

    public string? AttestationDeSalairePath { get; set; }

    public string? SituationProfessionnelle { get; set; }

    public decimal? RevenuMensuelNet { get; set; }

    public string? AutresSourcesDeRevenu { get; set; }

    public decimal? MontantMensuelAutresRevenus { get; set; }

    public string? Adresse { get; set; }

    public string? Pays { get; set; }

    public string? Gouvernorat { get; set; }

    public string? Ville { get; set; }

    public string? CodePostal { get; set; }

    public virtual DemandeCreditImmobilier? DemandeCreditImmobilier { get; set; }

    public virtual DemandeCreditVehicule? DemandeCreditVehicule { get; set; }

    public virtual Client? IdClientNavigation { get; set; }

    public virtual Compte? IdCompteNavigation { get; set; }

    public virtual Credit? IdCreditNavigation { get; set; }
}
