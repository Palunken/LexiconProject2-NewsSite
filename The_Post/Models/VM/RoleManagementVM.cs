using Microsoft.AspNetCore.Identity;

namespace The_Post.Models.VM
{
    public class RoleManagementVM
    {
        public List<IdentityRole> Roles { get; set; }        
        public AssignRoleVM AssignRoleVM { get; set; }
    }
}
