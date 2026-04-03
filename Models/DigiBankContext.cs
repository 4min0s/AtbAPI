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

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Compte> Comptes { get; set; }

    public virtual DbSet<Credit> Credits { get; set; }

    public virtual DbSet<DemandeCompte> DemandeComptes { get; set; }

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
            entity.Property(e => e.Cout)
                .HasPrecision(15, 2)
                .HasColumnName("cout");
            entity.Property(e => e.DateDeblocage).HasColumnName("date_deblocage");
            entity.Property(e => e.DeposeDate).HasColumnName("depose_date");
            entity.Property(e => e.Duree).HasColumnName("duree");
            entity.Property(e => e.Echeance).HasColumnName("echeance");
            entity.Property(e => e.Franchise)
                .HasDefaultValue(false)
                .HasColumnName("franchise");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.IdCompte).HasColumnName("id_compte");
            entity.Property(e => e.Montant)
                .HasPrecision(15, 2)
                .HasColumnName("montant");
            entity.Property(e => e.MontantTotalARembourser)
                .HasPrecision(15, 2)
                .HasColumnName("montant_total_a_rembourser");
            entity.Property(e => e.NbEcheance).HasColumnName("nb_echeance");
            entity.Property(e => e.Objet)
                .HasMaxLength(255)
                .HasColumnName("objet");
            entity.Property(e => e.Reference)
                .HasMaxLength(50)
                .HasColumnName("reference");
            entity.Property(e => e.Rythme).HasColumnName("rythme");
            entity.Property(e => e.Taux)
                .HasPrecision(5, 2)
                .HasColumnName("taux");
            entity.Property(e => e.TauxAssInc)
                .HasPrecision(5, 2)
                .HasColumnName("taux_ass_inc");
            entity.Property(e => e.TauxAssVie)
                .HasPrecision(5, 2)
                .HasColumnName("taux_ass_vie");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.EcheanceNavigation).WithMany(p => p.Credits)
                .HasForeignKey(d => d.Echeance)
                .HasConstraintName("credit_echeance_actuelle_fkey");

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

        modelBuilder.Entity<Echeance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("echeance_pkey");

            entity.ToTable("echeance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amortissement)
                .HasPrecision(15, 2)
                .HasColumnName("amortissement");
            entity.Property(e => e.AssInc)
                .HasPrecision(15, 2)
                .HasColumnName("ass_inc");
            entity.Property(e => e.AssVie)
                .HasPrecision(15, 2)
                .HasColumnName("ass_vie");
            entity.Property(e => e.CapitalRembourse)
                .HasPrecision(15, 2)
                .HasColumnName("capital_rembourse");
            entity.Property(e => e.CapitalRestant)
                .HasPrecision(15, 2)
                .HasColumnName("capital_restant");
            entity.Property(e => e.DateEcheance).HasColumnName("date_echeance");
            entity.Property(e => e.DatePaiement).HasColumnName("date_paiement");
            entity.Property(e => e.IdCredit).HasColumnName("id_credit");
            entity.Property(e => e.IntAdd)
                .HasPrecision(15, 2)
                .HasColumnName("int_add");
            entity.Property(e => e.Interet)
                .HasPrecision(15, 2)
                .HasColumnName("interet");
            entity.Property(e => e.InteretRembourse)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m)
                .HasColumnName("interet_rembourse");
            entity.Property(e => e.InteretRestant)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m)
                .HasColumnName("interet_restant");
            entity.Property(e => e.NumeroEcheance).HasColumnName("numero_echeance");
            entity.Property(e => e.Statut)
                .HasDefaultValue((short)0)
                .HasColumnName("statut");
            entity.Property(e => e.TotalEcheance)
                .HasPrecision(15, 2)
                .HasColumnName("total_echeance");

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
