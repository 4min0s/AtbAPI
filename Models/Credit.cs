using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Credit
{
    public int Id { get; set; }

    public int? IdClient { get; set; }

    public int? IdCompte { get; set; }

    public string? Reference { get; set; }

    public decimal? Cout { get; set; }

    public string? Objet { get; set; }

    public string? Type { get; set; }

    public decimal? Montant { get; set; }

    public int? Duree { get; set; }

    public int? NbEcheance { get; set; }

    public DateOnly? DeposeDate { get; set; }

    public DateOnly? DateDeblocage { get; set; }

    public bool? Franchise { get; set; }

    public decimal? Taux { get; set; }

    public decimal? TauxAssVie { get; set; }

    public decimal? TauxAssInc { get; set; }

    public short? Rythme { get; set; }

    public int? Echeance { get; set; }

    public decimal? MontantTotalARembourser { get; set; }

    public virtual Echeance? EcheanceNavigation { get; set; }

    public virtual ICollection<Echeance> Echeances { get; set; } = new List<Echeance>();

    public virtual Client? IdClientNavigation { get; set; }

    public virtual Compte? IdCompteNavigation { get; set; }
}
