using System.ComponentModel.DataAnnotations;

namespace The_Post.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Date Stamp")]
        public DateTime DateStamp { get; set; } = DateTime.Now;

        [Required]
        [StringLength(300)]
        public string LinkText { get; set; }

        [Required]
        [Display(Name = "Editors Choice")]
        public bool EditorsChoice { get; set; }

        [Required]
        [StringLength(200)]
        public string HeadLine { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name ="Content Summary")]
        public string ContentSummary { get; set; }

        [Required]
        [StringLength(10000)]
        public string Content { get; set; }

        [Required]
        public int Views { get; set; } = 0;

        [Required]
        [StringLength(500)]
        public string ImageOriginalLink { get; set; }
        public string? ImageMediumLink
        {
            get
            {
                return ImageOriginalLink?.Replace("articleimages/", "articleimages-md/");
            }
        }
        public string? ImageSmallLink 
        {
            get
            {
                return ImageOriginalLink?.Replace("articleimages/", "articleimages-sm/");
            }
        }

        [Required]
        public bool IsArchived { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}

    

