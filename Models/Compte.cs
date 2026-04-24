using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Compte
{
    public int Id { get; set; }

    public int? IdClient { get; set; }

    public decimal? Solde { get; set; }

    public string? TypeCompte { get; set; }

    public string? Rib { get; set; }

    public DateOnly? DateOuverture { get; set; }

    public string? CodeSwift { get; set; }

    public decimal? TauxInteretCrediteur { get; set; }

    public decimal? InteretCrediteurs { get; set; }

    public decimal? TauxInteretDebiteur { get; set; }

    public decimal? InteretDebiteur { get; set; }

    public decimal? SoldeDisponible { get; set; }

    public short? FrequenceDesInterets { get; set; }

    public string? DeviseCompte { get; set; }

    public short? Pack { get; set; }

    public int? IdAgence { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    public virtual ICollection<DemandeCredit> DemandeCredits { get; set; } = new List<DemandeCredit>();

    public virtual Agence? IdAgenceNavigation { get; set; }

    public virtual Client? IdClientNavigation { get; set; }
}
