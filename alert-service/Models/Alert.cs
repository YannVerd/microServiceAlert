using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alert_service.Models
{
    [Table("alert")]
    public class Alert
    {
        [Key]
        [Column("id_alert")]
        public int IdAlert { get; set; }

        [Required]
        [ForeignKey("EntityDetails")]
        [Column("id_entity_details")]
        public int IdEntityDetails {get; set;}
        public EntityDetails? EntityDetails { get; set; }

        public  ICollection<Notify>? Notifys {get; set;}
      
    }
}
