using Microsoft.AspNetCore.Identity;
using The_Post.Models;
using The_Post.Models.VM;

namespace The_Post.Services
{
    public interface IEmployeeService
    {
        public Task<IdentityResult> AddEmployee(User user, string password);
        public Task AssignRole(string userId, string role);        
        public Task<bool> DeleteEmployee(string userId);
        public Task<bool> EditEmployee(EditEmployeeVM employeeVM);
        public Task<List<User>> GetAllEmployees();
        public Task<List<AllEmployeesVM>> GetAllEmployeesWithRolesAsync();
        public Task<EditEmployeeVM?> GetEmployeeById(string userId);
    }
}
