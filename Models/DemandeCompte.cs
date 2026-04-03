using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class DemandeCompte
{
    public int Id { get; set; }

    public int? IdClient { get; set; }

    public DateOnly? DateEnvoi { get; set; }

    public string? TypeCompte { get; set; }

    public int? Etat { get; set; }

    public string? DemandePdfPath { get; set; }

    public string? Reference { get; set; }

    public int? IdAgence { get; set; }

    public string? Statut { get; set; }

    public string? Telephone { get; set; }

    public string? Adresse { get; set; }

    public string? Gouvernorat { get; set; }

    public string? Civilite { get; set; }

    public string? Ville { get; set; }

    public string? CodePostal { get; set; }

    public string? Profession { get; set; }

    public string? NomEmployeur { get; set; }

    public string? Devise { get; set; }

    public decimal? RevenuMensuel { get; set; }

    public bool? RelationBanque { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public DateOnly? DateNaissance { get; set; }

    public string? LieuNaissance { get; set; }

    public bool? Sexe { get; set; }

    public string? Cin { get; set; }

    public DateOnly? DateDelivrance { get; set; }

    public string? Pays { get; set; }

    public string? CinPathFront { get; set; }

    public string? CinPathBack { get; set; }

    public string? IndicateurResidencePath { get; set; }

    public virtual Agence? IdAgenceNavigation { get; set; }

    public virtual Client? IdClientNavigation { get; set; }
}
