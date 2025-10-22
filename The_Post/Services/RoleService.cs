
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using The_Post.Models;

namespace The_Post.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task AddRoleAsync(string roleName)
        {
          if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole (roleName));    
            }
        }

        public async Task DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync (roleId);
            if (role == null)
            {
                throw new ArgumentException("Role not found", nameof(roleId));
            }
            await _roleManager.DeleteAsync(role);
        }

        public async Task EditRoleAsync(IdentityRole role)
        {           
            await _roleManager.UpdateAsync(role);            
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync() 
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole?> GetRoleByIdAsync(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("Role ID cannot be null or empty.", nameof(roleId));
            }

            return await _roleManager.FindByIdAsync(roleId); 
        }
    }
}
