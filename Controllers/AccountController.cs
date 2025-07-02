#nullable enable

using CampusLink.Models;
using CampusLink_Application.Data;
using CampusLink_Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CampusLink_Application.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _context;

        // ✅ FIX: Inject _context in the constructor
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            ViewBag.Roles = new List<string> { "Student", "Lecturer" };
            ViewBag.Departments = _context.Departments.Select(d => d.DepartmentName).ToList();
            
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Roles = new List<string> { "Student", "Lecturer" };
            ViewBag.Departments = _context.Departments.Select(d => d.DepartmentName).ToList();

            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            // ✅ First create the user account
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // ✅ Role logic: Determine what role the user selected
            if (model.Role == "Student")
            {
                var student = new Student
                {
                    RegNo = model.RegistrationNumber
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                user.StudentId = student.Id;
                await _userManager.UpdateAsync(user); // ✅ Save link
                await _userManager.AddToRoleAsync(user, "Student");
            }
            else if (model.Role == "Lecturer")
            {
                // ✅ Get the department object first
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentName == model.Department);

                if (department == null)
                {
                    ModelState.AddModelError("", "Invalid department selected.");
                    return View(model);
                }

                var lecturer = new Lecturer
                {
                    DepartmentId = department.Id
                };

                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();

                user.LecturerId = lecturer.Id;
                await _userManager.UpdateAsync(user); // ✅ Save link
                await _userManager.AddToRoleAsync(user, "Lecturer");
            }
            if (model.Role == "Admin")
            {

            }
                // ✅ Send confirmation email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(
                "ConfirmEmail", "Account",
                new { userId = user.Id, token },
                protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,
                "CampusLink Email Confirmation",
                $"Please confirm your email by <a href='{confirmationLink}'>clicking here</a>.");

            TempData["Success"] = "Account created successfully! Please check your email to confirm.";
            return RedirectToAction("Login");
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Success"] = "Email confirmed! You can now log in.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Email confirmation failed.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LogInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Invalid login or email not confirmed.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
