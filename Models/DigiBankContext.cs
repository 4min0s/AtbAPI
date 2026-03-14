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

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Compte> Comptes { get; set; }

    public virtual DbSet<DemandeCompte> DemandeComptes { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=DigiBank;Username=postgres;Password=admin");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.Solde)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m)
                .HasColumnName("solde");
            entity.Property(e => e.TypeCompte)
                .HasMaxLength(20)
                .HasColumnName("type_compte");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Comptes)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("compte_id_client_fkey");
        });

        modelBuilder.Entity<DemandeCompte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("demande_compte_pkey");

            entity.ToTable("demande_compte");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateEnvoi).HasColumnName("date_envoi");
            entity.Property(e => e.DemandePdfPath).HasColumnName("demande_pdf_path");
            entity.Property(e => e.Etat).HasColumnName("etat");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.TypeCompte)
                .HasMaxLength(50)
                .HasColumnName("type_compte");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.DemandeComptes)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("demande_compte_id_client_fkey");
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
