using CampusLink_Application.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminSettingsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public AdminSettingsController(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    public IActionResult Index()
    {
        var model = new SettingsViewModel
        {
            SiteTitle = "CampusLink",
            InstitutionName = "Your Institution",
            MaxLoginAttempts = 5,
            AllowUserRegistration = true,
            Enable2FA = true,
            SessionTimeoutMinutes = 20,
            SendEmailOnNewUser = true,
            SupportEmail = "support@campuslink.com",
            ThemeColor = "blue"
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(SettingsViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Save settings to database or appsettings.json based on your setup
        TempData["Success"] = "Settings saved successfully!";
        return RedirectToAction("Index");
    }
}
