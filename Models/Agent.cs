using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Agent
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MotDePasse { get; set; } = null!;

    public int IdAgence { get; set; }

    public virtual Agence IdAgenceNavigation { get; set; } = null!;
}
