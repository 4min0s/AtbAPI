using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Echeance
{
    public int Id { get; set; }

    public int? IdCredit { get; set; }

    public DateOnly? DatePaiement { get; set; }

    public int NumeroEcheance { get; set; }

    public decimal? Interet { get; set; }

    public decimal? CapitalRembourse { get; set; }

    public decimal? CapitalRestant { get; set; }

    public decimal? Frais { get; set; }

    public decimal? EcheanceMontant { get; set; }

    public short? Etat { get; set; }

    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    public virtual Credit? IdCreditNavigation { get; set; }
}
