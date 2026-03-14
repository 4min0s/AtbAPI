using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Compte
{
    public int Id { get; set; }

    public int? IdClient { get; set; }

    public decimal? Solde { get; set; }

    public string? TypeCompte { get; set; }

    public virtual Client? IdClientNavigation { get; set; }
}
