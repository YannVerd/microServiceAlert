using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace alert_service.Models
{
    [Table("notify")]
    public class Notify
    {
        [Key]
        [Column("id_notify")]
        public int IdNotify{ get; set; }

        [Required]
        [ForeignKey("IdAlert")]
        [Column("id_alert")]
        public int IdAlert { get; set; }
        public Alert? Alert {get; set;}
        
        [Required]
        [Column("id_DM_user")]
        public string? IdDMUser{ get; set; }    
               
    }
}
