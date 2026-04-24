using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class DemandeCreditImmobilier
{
    public int Id { get; set; }

    public int IdDemande { get; set; }

    public string SousType { get; set; } = null!;

    public virtual DemandeCreditImmobilierAcquisition? DemandeCreditImmobilierAcquisition { get; set; }

    public virtual DemandeCreditImmobilierConstruction? DemandeCreditImmobilierConstruction { get; set; }

    public virtual DemandeCreditImmobilierRenovation? DemandeCreditImmobilierRenovation { get; set; }

    public virtual DemandeCredit IdDemandeNavigation { get; set; } = null!;
}
