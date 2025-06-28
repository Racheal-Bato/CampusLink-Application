using CampusLink_Application.Data;
using CampusLink_Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusLink_Application.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // Optional: Confirm password matches
            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return View(user);
            }

            // Save user to database (you can hash the password here if needed)
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Account created successfully!";
            return RedirectToAction("LogIn", "Account"); // Redirect to login or another page
        }
    }
}
