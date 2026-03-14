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

    public virtual Client? IdClientNavigation { get; set; }
}
