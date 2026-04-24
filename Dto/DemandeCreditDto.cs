namespace TodoApi.DTOs
{
    public class DemandeCreditDto
    {
        // ── Common fields ──────────────────────────────
        public int IdClient { get; set; }
        public int IdCompte { get; set; }
        public string TypeCredit { get; set; } = null!; // 'consommation' | 'vehicule' | 'immobilier'
        public string? NatureCredit { get; set; }
        public string? Objet { get; set; }
        public decimal? Montant { get; set; }
        public int? DureeMois { get; set; }
        public short? Periodicite { get; set; }
        public int? DureeGrace { get; set; }

        // -- Adresse 
        public string? Adresse { get; set; }

        public string? Pays { get; set; }

        public string? Gouvernorat { get; set; }

        public string? Ville { get; set; }

        public string? CodePostal { get; set; }



        // ── Financial profile ──────────────────────────
        public string? SituationProfessionnelle { get; set; }
        public decimal? RevenuMensuelNet { get; set; }
        public string? AutresSourcesDeRevenu { get; set; }
        public decimal? MontantMensuelAutresRevenus { get; set; }

        // ── Document paths (all types) ─────────────────
        public string? CinPathFront { get; set; }
        public string? CinPathBack { get; set; }
        public string? IndicateurResidencePath { get; set; }
        public string? FicheDePaiePath { get; set; }
        public string? AttestationDeTravailPath { get; set; }
        public string? AttestationDeSalairePath { get; set; }

        // ── Vehicule fields (only if TypeCredit = 'vehicule') ──
        public decimal? PrixVehicule { get; set; }
        public int? PuissanceFiscale { get; set; }
        public bool? VehiculeNeuf { get; set; }
        // if neuf = true
        public string? FactureProformaPath { get; set; }
        // if neuf = false
        public DateOnly? DatePremiereMiseEnCirculation { get; set; }
        public string? CarteGrisePath { get; set; }
        public string? PromesseVentePath { get; set; }

        // ── Immobilier fields (only if TypeCredit = 'immobilier') ──
        public string? SousType { get; set; } // 'acquisition' | 'construction' | 'renovation'
        // acquisition
        public decimal? PrixImmobilier { get; set; }
        public string? TypeAcquisition { get; set; }
        public string? PromesseVenteImmobilierPath { get; set; }
        // construction
        public decimal? CoutTravaux { get; set; }
        public string? DevisEstimatifPath { get; set; }
        public string? AutorisationBatirPath { get; set; }
        public string? PlanArchitectePath { get; set; }
        // renovation
        public string? DevisEstimatifRenovationPath { get; set; }
        public decimal? CoutTravauxRenovation { get; set; }
    }
}