public class PdfRequestDto
{
    // Simulation inputs
    public double DurationYears { get; set; }
    public double DurationMonths { get; set; }
    public double Montant { get; set; }
    public double TauxAnnuel { get; set; }
    public int Periodicite { get; set; }
    public int DureeGrace { get; set; }

    // PDF metadata
    public string ReferenceCredit { get; set; }
    public string NCompte { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string NatureCredit { get; set; }
    public double Teg { get; set; }
    public double Tiex { get; set; }
    public double Tmm { get; set; }
    public double MargeBanque { get; set; }
    public double TauxInteret { get; set; }
    public int DureeMonths { get; set; }
    public int DelaisGrace { get; set; }
    public DateTime DatePremEcheance { get; set; }
    public string PeriodiciteInt { get; set; }
}