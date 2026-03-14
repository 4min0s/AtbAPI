using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Client
{
    public int Id { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? NumTel { get; set; }

    public DateOnly? DateNaissance { get; set; }

    public string? LieuNaissance { get; set; }

    public bool? Sexe { get; set; }

    public bool? Civilite { get; set; }

    public string? Profession { get; set; }

    public string? NomEmployeur { get; set; }

    public decimal? MontantRevMensuelNet { get; set; }

    public string? Devise { get; set; }

    public bool? RelationBanque { get; set; }

    public string? Cin { get; set; }

    public DateOnly? DateDelivrance { get; set; }

    public string? Adresse { get; set; }

    public string? Pays { get; set; }

    public string? Gouvernorat { get; set; }

    public string? Ville { get; set; }

    public string? CodePostal { get; set; }

    public string? CinPathFront { get; set; }

    public string? CinPathBack { get; set; }

    public string? IndicateurResidencePath { get; set; }

    public virtual ICollection<Compte> Comptes { get; set; } = new List<Compte>();

    public virtual ICollection<DemandeCompte> DemandeComptes { get; set; } = new List<DemandeCompte>();

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
