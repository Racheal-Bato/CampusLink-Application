using CampusLink_Application.Data;
using CampusLink_Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusLink_Application.Controllers
{
    public class AccountController : Controller
    {
       
        // GET: /Account/Login
        public IActionResult LogIn()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult LogIn(Account model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Dummy logic: Replace this with your actual authentication code
            if (model.Email == "admin@example.com" && model.Password == "password123")
            {
                // Normally you'd sign in the user here using ASP.NET Core Identity
                TempData["Success"] = "Login successful!";
                return RedirectToAction("Index", "Home"); // or wherever you want to go after login
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        // Optional: Log out method
        public IActionResult Logout()
        {
            // Sign out logic goes here
            return RedirectToAction("LogIn");
        }
    }
}

