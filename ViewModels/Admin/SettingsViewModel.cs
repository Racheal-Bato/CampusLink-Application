namespace CampusLink_Application.ViewModels.Admin
{
    public class SettingsViewModel
    {
        // System Settings
        public string SiteTitle { get; set; }
        public string InstitutionName { get; set; }

        // User Settings
        public int MaxLoginAttempts { get; set; }
        public bool AllowUserRegistration { get; set; }

        // Security Settings
        public bool Enable2FA { get; set; }
        public int SessionTimeoutMinutes { get; set; }

        // Notification Settings
        public bool SendEmailOnNewUser { get; set; }
        public string SupportEmail { get; set; }

        // Appearance Settings
        public string ThemeColor { get; set; }
    }
}
