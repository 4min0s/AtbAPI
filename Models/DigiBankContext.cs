using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models;

public partial class DigiBankContext : DbContext
{
    public DigiBankContext()
    {
    }

    public DigiBankContext(DbContextOptions<DigiBankContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agence> Agences { get; set; }

    public virtual DbSet<Agent> Agents { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Compte> Comptes { get; set; }

    public virtual DbSet<Credit> Credits { get; set; }

    public virtual DbSet<DemandeCompte> DemandeComptes { get; set; }

    public virtual DbSet<DemandeCredit> DemandeCredits { get; set; }

    public virtual DbSet<DemandeCreditImmobilier> DemandeCreditImmobiliers { get; set; }

    public virtual DbSet<DemandeCreditImmobilierAcquisition> DemandeCreditImmobilierAcquisitions { get; set; }

    public virtual DbSet<DemandeCreditImmobilierConstruction> DemandeCreditImmobilierConstructions { get; set; }

    public virtual DbSet<DemandeCreditImmobilierRenovation> DemandeCreditImmobilierRenovations { get; set; }

    public virtual DbSet<DemandeCreditVehicule> DemandeCreditVehicules { get; set; }

    public virtual DbSet<Echeance> Echeances { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=DigiBank;Username=postgres;Password=admin");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agence>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agence_pkey");

            entity.ToTable("agence");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.CodePostal)
                .HasMaxLength(10)
                .HasColumnName("code_postal");
            entity.Property(e => e.Fax)
                .HasMaxLength(20)
                .HasColumnName("fax");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .HasColumnName("telephone");
        });

        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agent_pkey");

            entity.ToTable("agent");

            entity.HasIndex(e => e.Email, "agent_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.IdAgence).HasColumnName("id_agence");
            entity.Property(e => e.MotDePasse)
                .HasMaxLength(255)
                .HasColumnName("mot_de_passe");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.Prenom)
                .HasMaxLength(100)
                .HasColumnName("prenom");

            entity.HasOne(d => d.IdAgenceNavigation).WithMany(p => p.Agents)
                .HasForeignKey(d => d.IdAgence)
                .HasConstraintName("fk_agent_agence");
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("card_pkey");

            entity.ToTable("card");

            entity.HasIndex(e => e.NumeroCard, "card_numero_card_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateEmission)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date_emission");
            entity.Property(e => e.DateExpiration).HasColumnName("date_expiration");
            entity.Property(e => e.Disponible)
                .HasPrecision(15, 2)
                .HasColumnName("disponible");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.IdCompte).HasColumnName("id_compte");
            entity.Property(e => e.NumeroCard)
                .HasMaxLength(20)
                .HasColumnName("numero_card");
            entity.Property(e => e.Plafond)
                .HasPrecision(15, 2)
                .HasColumnName("plafond");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("card_id_client_fkey");

            entity.HasOne(d => d.IdCompteNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.IdCompte)
                .HasConstraintName("card_id_compte_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("client_pkey");

            entity.ToTable("client");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.Cin)
                .HasMaxLength(20)
                .HasColumnName("cin");
            entity.Property(e => e.CinPathBack).HasColumnName("cin_path_back");
            entity.Property(e => e.CinPathFront).HasColumnName("cin_path_front");
            entity.Property(e => e.Civilite).HasColumnName("civilite");
            entity.Property(e => e.CodePostal)
                .HasMaxLength(20)
                .HasColumnName("code_postal");
            entity.Property(e => e.DateDelivrance).HasColumnName("date_delivrance");
            entity.Property(e => e.DateNaissance).HasColumnName("date_naissance");
            entity.Property(e => e.Devise)
                .HasMaxLength(10)
                .HasColumnName("devise");
            entity.Property(e => e.Gouvernorat)
                .HasMaxLength(100)
                .HasColumnName("gouvernorat");
            entity.Property(e => e.IndicateurResidencePath).HasColumnName("indicateur_residence_path");
            entity.Property(e => e.LieuNaissance)
                .HasMaxLength(100)
                .HasColumnName("lieu_naissance");
            entity.Property(e => e.MontantRevMensuelNet)
                .HasPrecision(12, 2)
                .HasColumnName("montant_rev_mensuel_net");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.NomEmployeur)
                .HasMaxLength(100)
                .HasColumnName("nom_employeur");
            entity.Property(e => e.NumTel)
                .HasMaxLength(20)
                .HasColumnName("num_tel");
            entity.Property(e => e.Pays)
                .HasMaxLength(100)
                .HasColumnName("pays");
            entity.Property(e => e.Prenom)
                .HasMaxLength(100)
                .HasColumnName("prenom");
            entity.Property(e => e.Profession)
                .HasMaxLength(100)
                .HasColumnName("profession");
            entity.Property(e => e.RelationBanque).HasColumnName("relation_banque");
            entity.Property(e => e.Sexe).HasColumnName("sexe");
            entity.Property(e => e.Ville)
                .HasMaxLength(100)
                .HasColumnName("ville");
        });

        modelBuilder.Entity<Compte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("compte_pkey");

            entity.ToTable("compte");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CodeSwift)
                .HasMaxLength(20)
                .HasDefaultValueSql("'ATBKTNTT'::character varying")
                .HasColumnName("code_swift");
            entity.Property(e => e.DateOuverture).HasColumnName("date_ouverture");
            entity.Property(e => e.DeviseCompte)
                .HasMaxLength(10)
                .HasColumnName("devise_compte");
            entity.Property(e => e.FrequenceDesInterets).HasColumnName("frequence_des_interets");
            entity.Property(e => e.IdAgence).HasColumnName("id_agence");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.InteretCrediteurs)
                .HasPrecision(15, 2)
                .HasColumnName("interet_crediteurs");
            entity.Property(e => e.InteretDebiteur)
                .HasPrecision(15, 2)
                .HasColumnName("interet_debiteur");
            entity.Property(e => e.Pack).HasColumnName("pack");
            entity.Property(e => e.Rib)
                .HasMaxLength(20)
                .HasColumnName("rib");
            entity.Property(e => e.Solde)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m)
                .HasColumnName("solde");
            entity.Property(e => e.SoldeDisponible)
                .HasPrecision(15, 2)
                .HasColumnName("solde_disponible");
            entity.Property(e => e.TauxInteretCrediteur)
                .HasPrecision(5, 2)
                .HasColumnName("taux_interet_crediteur");
            entity.Property(e => e.TauxInteretDebiteur)
                .HasPrecision(5, 2)
                .HasColumnName("taux_interet_debiteur");
            entity.Property(e => e.TypeCompte)
                .HasMaxLength(20)
                .HasColumnName("type_compte");

            entity.HasOne(d => d.IdAgenceNavigation).WithMany(p => p.Comptes)
                .HasForeignKey(d => d.IdAgence)
                .HasConstraintName("compte_id_agence_fkey");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Comptes)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("compte_id_client_fkey");
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("credit_pkey");

            entity.ToTable("credit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CurrentEcheance).HasColumnName("current_echeance");
            entity.Property(e => e.DateDeblocage).HasColumnName("date_deblocage");
            entity.Property(e => e.DateDerniereEcheance).HasColumnName("date_derniere_echeance");
            entity.Property(e => e.DureeGrace)
                .HasDefaultValue(0)
                .HasColumnName("duree_grace");
            entity.Property(e => e.DureeMois).HasColumnName("duree_mois");
            entity.Property(e => e.FraisAdditionel)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m)
                .HasColumnName("frais_additionel");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.IdCompte).HasColumnName("id_compte");
            entity.Property(e => e.MargeBanque)
                .HasPrecision(5, 2)
                .HasColumnName("marge_banque");
            entity.Property(e => e.Montant)
                .HasPrecision(15, 2)
                .HasColumnName("montant");
            entity.Property(e => e.MontantRembourse)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m)
                .HasColumnName("montant_rembourse");
            entity.Property(e => e.MontantTotalARembourser)
                .HasPrecision(15, 2)
                .HasColumnName("montant_total_a_rembourser");
            entity.Property(e => e.NatureCredit)
                .HasMaxLength(100)
                .HasColumnName("nature_credit");
            entity.Property(e => e.NbEcheance).HasColumnName("nb_echeance");
            entity.Property(e => e.Objet)
                .HasMaxLength(255)
                .HasColumnName("objet");
            entity.Property(e => e.Periodicite).HasColumnName("periodicite");
            entity.Property(e => e.PremierEcheance).HasColumnName("premier_echeance");
            entity.Property(e => e.Reference)
                .HasMaxLength(50)
                .HasColumnName("reference");
            entity.Property(e => e.TabAmortissementPath).HasColumnName("tab_amortissement_path");
            entity.Property(e => e.TauxInteret)
                .HasPrecision(5, 2)
                .HasColumnName("taux_interet");
            entity.Property(e => e.Tmm)
                .HasPrecision(5, 2)
                .HasColumnName("tmm");

            entity.HasOne(d => d.CurrentEcheanceNavigation).WithMany(p => p.Credits)
                .HasForeignKey(d => d.CurrentEcheance)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("credit_current_echeance_fkey");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Credits)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("credit_id_client_fkey");

            entity.HasOne(d => d.IdCompteNavigation).WithMany(p => p.Credits)
                .HasForeignKey(d => d.IdCompte)
                .HasConstraintName("credit_id_compte_fkey");
        });

        modelBuilder.Entity<DemandeCompte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("demande_compte_pkey");

            entity.ToTable("demande_compte");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.Cin)
                .HasMaxLength(20)
                .HasColumnName("cin");
            entity.Property(e => e.CinPathBack).HasColumnName("cin_path_back");
            entity.Property(e => e.CinPathFront).HasColumnName("cin_path_front");
            entity.Property(e => e.Civilite)
                .HasMaxLength(100)
                .HasColumnName("civilite");
            entity.Property(e => e.CodePostal)
                .HasMaxLength(10)
                .HasColumnName("code_postal");
            entity.Property(e => e.DateDelivrance).HasColumnName("date_delivrance");
            entity.Property(e => e.DateEnvoi).HasColumnName("date_envoi");
            entity.Property(e => e.DateNaissance).HasColumnName("date_naissance");
            entity.Property(e => e.DemandePdfPath).HasColumnName("demande_pdf_path");
            entity.Property(e => e.Devise)
                .HasMaxLength(100)
                .HasColumnName("devise");
            entity.Property(e => e.Etat)
                .HasDefaultValue(0)
                .HasColumnName("etat");
            entity.Property(e => e.Gouvernorat)
                .HasMaxLength(100)
                .HasColumnName("gouvernorat");
            entity.Property(e => e.IdAgence).HasColumnName("id_agence");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.IndicateurResidencePath).HasColumnName("indicateur_residence_path");
            entity.Property(e => e.LieuNaissance)
                .HasMaxLength(100)
                .HasColumnName("lieu_naissance");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.NomEmployeur)
                .HasMaxLength(100)
                .HasColumnName("nom_employeur");
            entity.Property(e => e.Pays)
                .HasMaxLength(100)
                .HasColumnName("pays");
            entity.Property(e => e.Prenom)
                .HasMaxLength(100)
                .HasColumnName("prenom");
            entity.Property(e => e.Profession)
                .HasMaxLength(100)
                .HasColumnName("profession");
            entity.Property(e => e.Reference)
                .HasMaxLength(50)
                .HasColumnName("reference");
            entity.Property(e => e.RelationBanque)
                .HasDefaultValue(false)
                .HasColumnName("relation_banque");
            entity.Property(e => e.RevenuMensuel)
                .HasPrecision(12, 2)
                .HasColumnName("revenu_mensuel");
            entity.Property(e => e.Sexe).HasColumnName("sexe");
            entity.Property(e => e.Statut)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Actif'::character varying")
                .HasColumnName("statut");
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .HasColumnName("telephone");
            entity.Property(e => e.TypeCompte)
                .HasMaxLength(50)
                .HasColumnName("type_compte");
            entity.Property(e => e.Ville)
                .HasMaxLength(100)
                .HasColumnName("ville");

            entity.HasOne(d => d.IdAgenceNavigation).WithMany(p => p.DemandeComptes)
                .HasForeignKey(d => d.IdAgence)
                .HasConstraintName("demande_compte_id_agence_fkey");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.DemandeComptes)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("demande_compte_id_client_fkey");
        });

        modelBuilder.Entity<DemandeCredit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("demande_credit_pkey");

            entity.ToTable("demande_credit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Adresse).HasColumnName("adresse");
            entity.Property(e => e.AttestationDeSalairePath).HasColumnName("attestation_de_salaire_path");
            entity.Property(e => e.AttestationDeTravailPath).HasColumnName("attestation_de_travail_path");
            entity.Property(e => e.AutresSourcesDeRevenu).HasColumnName("autres_sources_de_revenu");
            entity.Property(e => e.CinPathBack).HasColumnName("cin_path_back");
            entity.Property(e => e.CinPathFront).HasColumnName("cin_path_front");
            entity.Property(e => e.CodePostal)
                .HasMaxLength(10)
                .HasColumnName("code_postal");
            entity.Property(e => e.DateEnvoi)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date_envoi");
            entity.Property(e => e.DureeGrace)
                .HasDefaultValue(0)
                .HasColumnName("duree_grace");
            entity.Property(e => e.DureeMois).HasColumnName("duree_mois");
            entity.Property(e => e.Etat)
                .HasDefaultValue(0)
                .HasColumnName("etat");
            entity.Property(e => e.FicheDePaiePath).HasColumnName("fiche_de_paie_path");
            entity.Property(e => e.Gouvernorat)
                .HasMaxLength(100)
                .HasColumnName("gouvernorat");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.IdCompte).HasColumnName("id_compte");
            entity.Property(e => e.IdCredit).HasColumnName("id_credit");
            entity.Property(e => e.IndicateurResidencePath).HasColumnName("indicateur_residence_path");
            entity.Property(e => e.Montant)
                .HasPrecision(15, 2)
                .HasColumnName("montant");
            entity.Property(e => e.MontantMensuelAutresRevenus)
                .HasPrecision(12, 2)
                .HasColumnName("montant_mensuel_autres_revenus");
            entity.Property(e => e.NatureCredit)
                .HasMaxLength(100)
                .HasColumnName("nature_credit");
            entity.Property(e => e.Objet)
                .HasMaxLength(255)
                .HasColumnName("objet");
            entity.Property(e => e.Pays)
                .HasMaxLength(100)
                .HasColumnName("pays");
            entity.Property(e => e.Periodicite).HasColumnName("periodicite");
            entity.Property(e => e.Reference)
                .HasMaxLength(50)
                .HasColumnName("reference");
            entity.Property(e => e.RevenuMensuelNet)
                .HasPrecision(12, 2)
                .HasColumnName("revenu_mensuel_net");
            entity.Property(e => e.SituationProfessionnelle)
                .HasMaxLength(100)
                .HasColumnName("situation_professionnelle");
            entity.Property(e => e.TypeCredit)
                .HasMaxLength(20)
                .HasColumnName("type_credit");
            entity.Property(e => e.Ville)
                .HasMaxLength(100)
                .HasColumnName("ville");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.DemandeCredits)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("demande_credit_id_client_fkey");

            entity.HasOne(d => d.IdCompteNavigation).WithMany(p => p.DemandeCredits)
                .HasForeignKey(d => d.IdCompte)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("demande_credit_id_compte_fkey");

            entity.HasOne(d => d.IdCreditNavigation).WithMany(p => p.DemandeCredits)
                .HasForeignKey(d => d.IdCredit)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("demande_credit_id_credit_fkey");
        });

        modelBuilder.Entity<DemandeCreditImmobilier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("demande_credit_immobilier_pkey");

            entity.ToTable("demande_credit_immobilier");

            entity.HasIndex(e => e.IdDemande, "demande_credit_immobilier_id_demande_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdDemande).HasColumnName("id_demande");
            entity.Property(e => e.SousType)
                .HasMaxLength(20)
                .HasColumnName("sous_type");

            entity.HasOne(d => d.IdDemandeNavigation).WithOne(p => p.DemandeCreditImmobilier)
                .HasForeignKey<DemandeCreditImmobilier>(d => d.IdDemande)
                .HasConstraintName("demande_credit_immobilier_id_demande_fkey");
        });

        modelBuilder.Entity<DemandeCreditImmobilierAcquisition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("demande_credit_immobilier_acquisition_pkey");

            entity.ToTable("demande_credit_immobilier_acquisition");

            entity.HasIndex(e => e.IdImmobilier, "demande_credit_immobilier_acquisition_id_immobilier_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdImmobilier).HasColumnName("id_immobilier");
            entity.Property(e => e.PrixImmobilier)
                .HasPrecision(15, 2)
                .HasColumnName("prix_immobilier");
            entity.Property(e => e.PromesseVentePath).HasColumnName("promesse_vente_path");
            entity.Property(e => e.TypeAcquisition)
                .HasMaxLength(100)
                .HasColumnName("type_acquisition");

            entity.HasOne(d => d.IdImmobilierNavigation).WithOne(p => p.DemandeCreditImmobilierAcquisition)
                .HasForeignKey<DemandeCreditImmobilierAcquisition>(d => d.IdImmobilier)
                .HasConstraintName("demande_credit_immobilier_acquisition_id_immobilier_fkey");
        });

        modelBuilder.Entity<DemandeCreditImmobilierConstruction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("demande_credit_immobilier_construction_pkey");

            entity.ToTable("demande_credit_immobilier_construction");

            entity.HasIndex(e => e.IdImmobilier, "demande_credit_immobilier_construction_id_immobilier_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutorisationBatirPath).HasColumnName("autorisation_batir_path");
            entity.Property(e => e.CoutTravaux)
                .HasPrecision(15, 2)
                .HasColumnName("cout_travaux");
            entity.Property(e => e.DevisEstimatifPath).HasColumnName("devis_estimatif_path");
            entity.Property(e => e.IdImmobilier).HasColumnName("id_immobilier");
            entity.Property(e => e.PlanArchitectePath).HasColumnName("plan_architecte_path");

            entity.HasOne(d => d.IdImmobilierNavigation).WithOne(p => p.DemandeCreditImmobilierConstruction)
                .HasForeignKey<DemandeCreditImmobilierConstruction>(d => d.IdImmobilier)
                .HasConstraintName("demande_credit_immobilier_construction_id_immobilier_fkey");
        });

        modelBuilder.Entity<DemandeCreditImmobilierRenovation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("demande_credit_immobilier_renovation_pkey");

            entity.ToTable("demande_credit_immobilier_renovation");

            entity.HasIndex(e => e.IdImmobilier, "demande_credit_immobilier_renovation_id_immobilier_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CoutTravaux)
                .HasPrecision(15, 2)
                .HasColumnName("cout_travaux");
            entity.Property(e => e.DevisEstimatifPath).HasColumnName("devis_estimatif_path");
            entity.Property(e => e.IdImmobilier).HasColumnName("id_immobilier");

            entity.HasOne(d => d.IdImmobilierNavigation).WithOne(p => p.DemandeCreditImmobilierRenovation)
                .HasForeignKey<DemandeCreditImmobilierRenovation>(d => d.IdImmobilier)
                .HasConstraintName("demande_credit_immobilier_renovation_id_immobilier_fkey");
        });

        modelBuilder.Entity<DemandeCreditVehicule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("demande_credit_vehicule_pkey");

            entity.ToTable("demande_credit_vehicule");

            entity.HasIndex(e => e.IdDemande, "demande_credit_vehicule_id_demande_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarteGrisePath).HasColumnName("carte_grise_path");
            entity.Property(e => e.DatePremiereMiseEnCirculation).HasColumnName("date_premiere_mise_en_circulation");
            entity.Property(e => e.FactureProformaPath).HasColumnName("facture_proforma_path");
            entity.Property(e => e.IdDemande).HasColumnName("id_demande");
            entity.Property(e => e.PrixVehicule)
                .HasPrecision(15, 2)
                .HasColumnName("prix_vehicule");
            entity.Property(e => e.PromesseVentePath).HasColumnName("promesse_vente_path");
            entity.Property(e => e.PuissanceFiscale).HasColumnName("puissance_fiscale");
            entity.Property(e => e.VehiculeNeuf).HasColumnName("vehicule_neuf");

            entity.HasOne(d => d.IdDemandeNavigation).WithOne(p => p.DemandeCreditVehicule)
                .HasForeignKey<DemandeCreditVehicule>(d => d.IdDemande)
                .HasConstraintName("demande_credit_vehicule_id_demande_fkey");
        });

        modelBuilder.Entity<Echeance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("echeance_pkey");

            entity.ToTable("echeance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CapitalRembourse)
                .HasPrecision(15, 2)
                .HasColumnName("capital_rembourse");
            entity.Property(e => e.CapitalRestant)
                .HasPrecision(15, 2)
                .HasColumnName("capital_restant");
            entity.Property(e => e.DatePaiement).HasColumnName("date_paiement");
            entity.Property(e => e.EcheanceMontant)
                .HasPrecision(15, 2)
                .HasColumnName("echeance_montant");
            entity.Property(e => e.Etat)
                .HasDefaultValue((short)0)
                .HasColumnName("etat");
            entity.Property(e => e.Frais)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m)
                .HasColumnName("frais");
            entity.Property(e => e.IdCredit).HasColumnName("id_credit");
            entity.Property(e => e.Interet)
                .HasPrecision(15, 2)
                .HasColumnName("interet");
            entity.Property(e => e.NumeroEcheance).HasColumnName("numero_echeance");

            entity.HasOne(d => d.IdCreditNavigation).WithMany(p => p.Echeances)
                .HasForeignKey(d => d.IdCredit)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("echeance_id_credit_fkey");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("profile_pkey");

            entity.ToTable("profile");

            entity.HasIndex(e => e.Email, "profile_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.MotDePasse)
                .HasMaxLength(255)
                .HasColumnName("mot_de_passe");
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .HasColumnName("otp");
            entity.Property(e => e.OtpExpiry)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("otp_expiry");

            entity.HasOne(d => d.Client).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_profile_client");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
