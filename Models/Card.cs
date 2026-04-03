using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Card
{
    public int Id { get; set; }

    public string NumeroCard { get; set; } = null!;

    public string? Type { get; set; }

    public int? IdClient { get; set; }

    public DateOnly? DateExpiration { get; set; }

    public DateOnly? DateEmission { get; set; }

    public string? Status { get; set; }

    public int? IdCompte { get; set; }

    public decimal? Plafond { get; set; }

    public decimal? Disponible { get; set; }

    public virtual Client? IdClientNavigation { get; set; }

    public virtual Compte? IdCompteNavigation { get; set; }
}
