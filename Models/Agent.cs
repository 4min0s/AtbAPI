using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("agent")]  
    public class Agent
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nom")]
        public string Nom { get; set; } = null!;

        [Column("prenom")]
        public string Prenom { get; set; } = null!;

        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("mot_de_passe")]
        public string MotDePasse { get; set; } = null!;

        [Column("id_agence")]
        public int IdAgence { get; set; }
    }
}