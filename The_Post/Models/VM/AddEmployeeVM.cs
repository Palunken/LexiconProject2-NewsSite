using System.ComponentModel.DataAnnotations;

namespace The_Post.Models.VM
{
    public class AddEmployeeVM
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
                
        [Required]
        public DateTime? DOB { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Zip { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
