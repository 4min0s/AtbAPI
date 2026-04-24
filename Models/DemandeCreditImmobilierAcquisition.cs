using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class DemandeCreditImmobilierAcquisition
{
    public int Id { get; set; }

    public int IdImmobilier { get; set; }

    public decimal? PrixImmobilier { get; set; }

    public string? TypeAcquisition { get; set; }

    public string? PromesseVentePath { get; set; }

    public virtual DemandeCreditImmobilier IdImmobilierNavigation { get; set; } = null!;
}
