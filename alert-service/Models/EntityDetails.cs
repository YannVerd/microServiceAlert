using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;


namespace alert_service.Models
{
    [Table("entitydetails")]
    public class EntityDetails
    {
        [Key]
        [Column("id_entity_details")]
        public int IdEntityDetails { get; set; }

        [Required]
        [Column("id_dm_entity")]
        public string? IdDMEntity { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(50)]
        public string? Name{ get; set; }

        [Required]
        [Column("type")]
        [MaxLength(50)]
        public string? Type{ get; set; }

        // necessaire pour les relations. A passer en [Required] si obligatoirement lié à une alert
        [Column("id_alert")]
        [ForeignKey("FK_id_alert")]
        public int? IdAlert {get; set;}
        public Alert? Alert {get; set;}


    }
}
