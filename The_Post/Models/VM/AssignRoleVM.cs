using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace The_Post.Models.VM
{
    public class AssignRoleVM
    {
        [Required(ErrorMessage = "User is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        // Lists for dropdowns
        public List<User> Users { get; set; } = new List<User>();
        public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
    }
}
