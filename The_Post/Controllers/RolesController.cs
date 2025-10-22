using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using The_Post.Models.VM;
using The_Post.Services;

namespace The_Post.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IEmployeeService _employeeService;

        public RolesController(IRoleService roleService, IEmployeeService employeeService)
        {
            _roleService = roleService;
            _employeeService = employeeService;
        }


        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            var employees = await _employeeService.GetAllEmployees();

            var model = new RoleManagementVM
            {
                Roles = roles,                
                AssignRoleVM = new AssignRoleVM
                {
                    Users = employees,
                    Roles = roles
                }
            };

            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["ErrorMessage"] = "Role name cannot be empty .";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                await _roleService.AddRoleAsync(roleName);
                TempData["SuccessMessage"] = "Role added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit (string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Role Not Found.";
                return RedirectToAction(nameof(Index));
            }
           return View(role);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            if (role == null || string.IsNullOrEmpty(role.Id))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(role);
            }
           
            var existingRole = await _roleService.GetRoleByIdAsync(role.Id);
            if (existingRole == null)
            {
                TempData["ErrorMessage"] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }

            existingRole.Name = role.Name;
            await _roleService.EditRoleAsync(existingRole);

            TempData["SuccessMessage"] = "Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                await _roleService.DeleteRoleAsync(id);
                TempData["SuccessMessage"] = "Role deleted successfully.";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole()
        {
            var model = new AssignRoleVM
            {
                Users = await _employeeService.GetAllEmployees(),
                Roles = await _roleService.GetAllRolesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(AssignRoleVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.AssignRole(model.UserId, model.Role);
                    TempData["SuccessMessage"] = "Role assigned successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            //Repopulate dropdowns if validation fails
            model.Users = await _employeeService.GetAllEmployees();
            model.Roles = await _roleService.GetAllRolesAsync();

            // Return view with errors
            var vM = new RoleManagementVM
            {
                Roles = await _roleService.GetAllRolesAsync(),                
                AssignRoleVM = model
            };

            return View("Index", vM);
        }
    }
}
