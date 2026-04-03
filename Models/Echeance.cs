using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Echeance
{
    public int Id { get; set; }

    public int? IdCredit { get; set; }

    public int NumeroEcheance { get; set; }

    public DateOnly DateEcheance { get; set; }

    public decimal? CapitalRembourse { get; set; }

    public decimal? CapitalRestant { get; set; }

    public decimal? Interet { get; set; }

    public decimal? Amortissement { get; set; }

    public decimal? AssVie { get; set; }

    public decimal? AssInc { get; set; }

    public decimal? IntAdd { get; set; }

    public decimal? TotalEcheance { get; set; }

    public short? Statut { get; set; }

    public DateOnly? DatePaiement { get; set; }

    public decimal? InteretRembourse { get; set; }

    public decimal? InteretRestant { get; set; }

    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    public virtual Credit? IdCreditNavigation { get; set; }
}
