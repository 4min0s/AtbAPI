using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TodoApi.Models
{
    [Table("demande_compte")]
    public partial class DemandeCompte
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_client")]
        public int? IdClient { get; set; }

        [Column("id_agence")]
        public int? IdAgence { get; set; }

        [Column("date_envoi")]
        public DateOnly? DateEnvoi { get; set; }

        [Column("type_compte")]
        public string? TypeCompte { get; set; }

        [Column("etat")]
        public int? Etat { get; set; }

        [Column("demande_pdf_path")]
        public string? DemandePdfPath { get; set; }

        [Column("reference")]
        public string? Reference { get; set; }

        [Column("statut")]
        public string? Statut { get; set; }

        [Column("adresse")]
        public string? Adresse { get; set; }

        [Column("gouvernorat")]
        public string? Gouvernorat { get; set; }

        [Column("civilite")]
        public string? Civilite { get; set; }

        [Column("ville")]
        public string? Ville { get; set; }

        [Column("code_postal")]
        public string? CodePostal { get; set; }

        [Column("profession")]
        public string? Profession { get; set; }

        [Column("nom_employeur")]
        public string? NomEmployeur { get; set; }

        [Column("devise")]
        public string? Devise { get; set; }

        [Column("revenu_mensuel")]
        public decimal? RevenuMensuel { get; set; }

        [Column("nom")]
        public string? Nom { get; set; }

        [Column("prenom")]
        public string? Prenom { get; set; }

        [Column("cin")]
        public string? Cin { get; set; }

        [Column("date_naissance")]
        public DateOnly? DateNaissance { get; set; }

        [Column("lieu_naissance")]
        public string? LieuNaissance { get; set; }

        [Column("sexe")]
        public bool? Sexe { get; set; }

        [Column("date_delivrance")]
        public DateOnly? DateDelivrance { get; set; }

        [Column("pays")]
        public string? Pays { get; set; }

        [Column("cin_path_front")]
        public string? CinPathFront { get; set; }

        [Column("cin_path_back")]
        public string? CinPathBack { get; set; }

        [Column("indicateur_residence_path")]
        public string? IndicateurResidencePath { get; set; }

        [Column("telephone")]
        public string? Telephone { get; set; }

        [Column("relation_banque")]
        public bool? RelationBanque { get; set; }

        // Navigation Properties
        [JsonIgnore]
        [ForeignKey("IdAgence")]
        public virtual Agence? IdAgenceNavigation { get; set; }

        [JsonIgnore]
        [ForeignKey("IdClient")]
        public virtual Client? IdClientNavigation { get; set; }
    }
}