using System.ComponentModel.DataAnnotations;

namespace The_Post.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
         public string Name { get; set; }


        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

        public ICollection<User> NewsletterUsers { get; set; } = new List<User>();
    }
}
