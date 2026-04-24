using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class DemandeCreditImmobilierConstruction
{
    public int Id { get; set; }

    public int IdImmobilier { get; set; }

    public decimal? CoutTravaux { get; set; }

    public string? DevisEstimatifPath { get; set; }

    public string? AutorisationBatirPath { get; set; }

    public string? PlanArchitectePath { get; set; }

    public virtual DemandeCreditImmobilier IdImmobilierNavigation { get; set; } = null!;
}
