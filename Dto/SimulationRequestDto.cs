public class SimulationRequestDto
{
    public double DurationMonths { get; set; }
    public double Montant { get; set; }
    public double TauxAnnuel { get; set; }
    public int Periodicite { get; set; }      // 1=Mensuel, 2=Trimestriel, 3=Semestriel
    public int DureeGrace { get; set; }
}