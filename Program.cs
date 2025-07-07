using CampusLink_Application.Data;
using CampusLink_Application.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CampusLink.Models;

var builder = WebApplication.CreateBuilder(args); // ? Declare builder first

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Lock out users after 5 failed attempts
    options.Lockout.MaxFailedAccessAttempts = 5;
    // Lockout duration: 20 minutes
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
    // Enable lockout for newly created users
    options.Lockout.AllowedForNewUsers = true;


}).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//This tells ASP.NET Identity to block login unless the user has confirmed their email.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();



builder.Services.AddControllersWithViews();

var app = builder.Build();



// This method seeds roles and a default admin user
async Task SeedRolesAndAdminAsync(IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create admin user if it doesn't exist
    string adminEmail = "admin@campuslink.com";
    string adminPassword = "Admin123!"; // Change to a secure password

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create admin user");
        }
        await userManager.AddToRoleAsync(adminUser, "Admin");

    }

    // Define the roles you want in your system
    string[] roles = { "Admin", "Lecturer", "Student" };

    // Create roles if they don't already exist
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Create a default admin user if it doesn't exist
    
    }


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdminAsync(app);

}



app.Run();
