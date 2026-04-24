using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Credit
{
    public int Id { get; set; }

    public int? IdClient { get; set; }

    public int? IdCompte { get; set; }

    public string? NatureCredit { get; set; }

    public string? Objet { get; set; }

    public string? Reference { get; set; }

    public decimal? Montant { get; set; }

    public int? DureeMois { get; set; }

    public int? NbEcheance { get; set; }

    public DateOnly? PremierEcheance { get; set; }

    public string? TabAmortissementPath { get; set; }

    public int? DureeGrace { get; set; }

    public decimal? Tmm { get; set; }

    public decimal? MargeBanque { get; set; }

    public decimal? TauxInteret { get; set; }

    public decimal? FraisAdditionel { get; set; }

    public short? Periodicite { get; set; }

    public decimal? MontantTotalARembourser { get; set; }

    public int? CurrentEcheance { get; set; }

    public decimal? MontantRembourse { get; set; }

    public DateOnly? DateDerniereEcheance { get; set; }

    public DateOnly? DateDeblocage { get; set; }

    public virtual Echeance? CurrentEcheanceNavigation { get; set; }

    public virtual ICollection<DemandeCredit> DemandeCredits { get; set; } = new List<DemandeCredit>();

    public virtual ICollection<Echeance> Echeances { get; set; } = new List<Echeance>();

    public virtual Client? IdClientNavigation { get; set; }

    public virtual Compte? IdCompteNavigation { get; set; }
}
