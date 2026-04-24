using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Agence
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string? Adresse { get; set; }

    public string? CodePostal { get; set; }

    public string? Telephone { get; set; }

    public string? Fax { get; set; }

    public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();

    public virtual ICollection<Compte> Comptes { get; set; } = new List<Compte>();

    public virtual ICollection<DemandeCompte> DemandeComptes { get; set; } = new List<DemandeCompte>();
}
