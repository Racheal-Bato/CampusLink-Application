using CampusLink_Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CampusLink_Application.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly List<string> _protectedRoles = new List<string> { "Admin", "Student" };


        public AdminRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Roles List
        public IActionResult List()
        {
            var roles = _roleManager.Roles.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            return View(roles);
        }

        // GET: Create Role
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create Role
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var roleExists = await _roleManager.RoleExistsAsync(model.Name);
            if (roleExists)
            {
                ModelState.AddModelError("", "Role already exists.");
                return View(model);
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
            if (result.Succeeded)
                return RedirectToAction("List");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // GET: Delete Role but first check for protected roles
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            // Prevent deletion of protected roles
            if (_protectedRoles.Contains(role.Name))
            {
                TempData["Error"] = $"The role '{role.Name}' is protected and cannot be deleted.";
                return RedirectToAction("List");
            }

            await _roleManager.DeleteAsync(role);
            TempData["Success"] = $"Role '{role.Name}' deleted successfully.";
            return RedirectToAction("List");
        }

    }
}
