#nullable enable

using CampusLink.Models;
using CampusLink_Application.Data;
using CampusLink_Application.Models;
using CampusLink_Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CampusLink_Application.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _context;

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
            ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer" });
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewBag.Departments = _context.Departments.Select(d => d.DepartmentName).ToList();

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer" });
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewBag.Departments = _context.Departments.Select(d => d.DepartmentName).ToList();

            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // Role-specific setup
            if (model.Role == "Student")
            {
                // Look up department from selected department name
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentName == model.Department);

                if (department == null)
                {
                    ModelState.AddModelError("", "Invalid department selected.");
                    return View(model);
                }

                var student = new Student
                {
                    Role = "Student",
                    CourseId = model.CourseId,
                    RegNo = model.RegistrationNumber,
                    DepartmentId = department.Id, // ✅ Correct department FK
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                user.StudentId = student.Id;
                await _userManager.UpdateAsync(user);
                await _userManager.AddToRoleAsync(user, "Student");
            }

            else if (model.Role == "Lecturer")
            {
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
                await _userManager.UpdateAsync(user);
                await _userManager.AddToRoleAsync(user, "Lecturer");
            }

            // Email Confirmation with 2FA Enabled after confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,
                "CampusLink Email Confirmation",
                $"Please confirm your email by <a href='{confirmationLink}'>clicking here</a>.");

            TempData["Success"] = "Account created successfully! Please check your email to confirm.";
            return RedirectToAction("Login");
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                TempData["Success"] = "Email confirmed! You can now log in.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Email confirmation failed.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LogInViewModel model, string? returnUrl = null)
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
                user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("Send2FACode", new { returnUrl, rememberMe = model.RememberMe });
            }

            if (result.Succeeded)
                return RedirectToLocal(returnUrl);

            if (result.IsLockedOut)
                return View("Lockout");

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // GET: /Account/Send2FACode
        [HttpGet]
        public async Task<IActionResult> Send2FACode(string? returnUrl, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return View("Error");

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            await _emailSender.SendEmailAsync(
                user.Email!,
                "Your 2FA Code",
                $"Your security code is: <strong>{token}</strong>");

            return View(new Verify2FAVm { ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/Send2FACode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send2FACode(Verify2FAVm model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return View("Error");

            var result = await _signInManager.TwoFactorSignInAsync(
                TokenOptions.DefaultEmailProvider,
                model.Code,
                model.RememberMe,
                rememberClient: false);

            if (result.Succeeded)
                return RedirectToLocal(model.ReturnUrl);

            if (result.IsLockedOut)
                return View("Lockout");

            ModelState.AddModelError("", "Invalid code.");
            return View(model);
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return RedirectToAction("ForgotPasswordConfirmation");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string? token, string? email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return RedirectToAction("Index", "Home");

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token!, model.Password);
            if (result.Succeeded)
                return RedirectToAction("Login");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPasswordConfirmationPost()
            => RedirectToAction("ForgotPasswordConfirmation");

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }

   
}
