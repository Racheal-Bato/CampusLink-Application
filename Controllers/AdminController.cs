using CampusLink.Models;
using CampusLink_Application.Models;
using CampusLink_Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction("Index");
    }

   
    // GET: Admin/Users/List?page=1

    public async Task<IActionResult> List(int? page)
    {
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var users = _userManager.Users.ToList();

        // Prepare a list of user info with roles
        var userRoleViewModels = new List<UserRoleViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoleViewModels.Add(new UserRoleViewModel
            {
                User = user,
                Roles = roles
            });
        }

        var pagedUsers = userRoleViewModels.ToPagedList(pageNumber, pageSize);

        return View(pagedUsers);
    }
    public async Task<IActionResult> AssignRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

        var viewModel = new AssignRolesViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            Roles = userRoles.ToList(),
            AllRoles = allRoles
        };

        return View(viewModel);
    }

    // POST: AdminUsers/AssignRoles
    [HttpPost]
    public async Task<IActionResult> AssignRoles(AssignRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);

        var rolesToAdd = model.Roles.Except(currentRoles);
        var rolesToRemove = currentRoles.Except(model.Roles);

        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        await _userManager.AddToRolesAsync(user, rolesToAdd);

        TempData["Success"] = "Roles updated successfully!";
        return RedirectToAction("List");
    }
    // LOCK USER
    public async Task<IActionResult> LockUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Lockout indefinitely
        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

        TempData["Success"] = "User account locked.";
        return RedirectToAction("List");
    }

    // UNLOCK USER
    public async Task<IActionResult> UnlockUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
        TempData["Success"] = "User account unlocked.";
        return RedirectToAction("List");
    }
    // GET: AdminUsers/Edit/{userId}
    public async Task<IActionResult> Edit(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var model = new Edit
        {
            Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            UserName = user.UserName
        };

        return View(model);
    }

    // POST: AdminUsers/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Edit model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
            return NotFound();

        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.UserName = model.UserName;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["Success"] = "User details updated successfully.";
            return RedirectToAction("List");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

}





