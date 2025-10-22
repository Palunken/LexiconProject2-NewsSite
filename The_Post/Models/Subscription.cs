using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace The_Post.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
                
        [Required]
        [Column("Price", TypeName = "decimal(18,2)")]
        public decimal HistoricalPrice { get; set; }

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Expires { get; set; }

        [Required]
        public bool PaymentComplete { get; set; }

        [Required]        
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int SubscriptionTypeId { get; set; }

        [ForeignKey("SubscriptionTypeId")]
        public virtual SubscriptionType SubscriptionType { get; set; }
    }
}
