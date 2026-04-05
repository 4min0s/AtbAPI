public class SimulateCreditRequestDto
{
    public double DurationYears { get; set; }
    public double Montant { get; set; }
    public double TauxAnnuel { get; set; }
    public int Periodicite { get; set; }        // 1=Mensuel, 2=Trimestriel, 3=Semestriel
    public int DureeGrace { get; set; }
    public double DurationMonths { get; set; }
}

public class SimulateCreditResponseDto
{
    public double NombreEcheances { get; set; }
    public double TotalInteret { get; set; }
    public double TotalCapital { get; set; }
    public double CoutTotalCredit { get; set;}
    public double Echenallance { get; set; }
}

public class GenerateAmortissementRequestDto
{
    public double DurationYears { get; set; }
    public double Montant { get; set; }
    public double TauxAnnuel { get; set; }
    public int Periodicite { get; set; }
    public string ReferenceCredit { get; set; } = string.Empty;
    public string NCompte { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NatureCredit { get; set; } = string.Empty;
    public double Teg { get; set; }
    public double Tiex { get; set; }
    public double Tmm { get; set; }
    public double MargeBanque { get; set; }
    public double TauxInteret { get; set; }
    public int DureeMonths { get; set; }
    public int DelaisGrace { get; set; }
    public DateTime DatePremEcheance { get; set; }
    public string PeriodiciteInt { get; set; } = string.Empty;
    public int DureeGrace { get; set; }
    public double DurationMonths { get; set; }
}

public class GenerateAmortissementResponseDto
{
    public string ReferenceCredit { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}