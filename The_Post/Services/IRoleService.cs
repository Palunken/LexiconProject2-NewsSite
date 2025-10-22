using Microsoft.AspNetCore.Identity;

namespace The_Post.Services
{
    public interface IRoleService
    {
        public Task AddRoleAsync(string roleName);        
        public Task EditRoleAsync(IdentityRole role);
        public Task DeleteRoleAsync(string roleId);
        public Task<List<IdentityRole>> GetAllRolesAsync();

        public Task<IdentityRole> GetRoleByIdAsync(string roleId);
    }
}
