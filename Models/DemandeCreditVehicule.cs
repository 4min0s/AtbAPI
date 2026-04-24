using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class DemandeCreditVehicule
{
    public int Id { get; set; }

    public int IdDemande { get; set; }

    public decimal? PrixVehicule { get; set; }

    public int? PuissanceFiscale { get; set; }

    public bool VehiculeNeuf { get; set; }

    public string? FactureProformaPath { get; set; }

    public DateOnly? DatePremiereMiseEnCirculation { get; set; }

    public string? CarteGrisePath { get; set; }

    public string? PromesseVentePath { get; set; }

    public virtual DemandeCredit IdDemandeNavigation { get; set; } = null!;
}
