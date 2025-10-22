using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace The_Post.Models
{
    public class User : IdentityUser
    {
        [Display(Name ="First Name")]
        [Required,MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        [Required, MaxLength(100)]
        public string LastName { get; set; }= string.Empty;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [StringLength(50, ErrorMessage = "Address can't be longer than 50 characters.")]        
        public string? Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City can't be longer than 50 characters.")]        
        public string City { get; set; }

        [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip must be a 5-digit number.")]
        public string? Zip { get; set; }

        public bool IsEmployee { get; set; }

        public bool EditorsChoiceNewsletter { get; set; }

        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<Like> Likes { get; set; } = [];
        public string? WeatherCities { get; set; }  // CSV (comma-separated values) string
        public ICollection<Category> NewsletterCategories { get; set; } = new List<Category>();
        public bool IsSubscribedToNewsletter { get; set; }
    }
}
