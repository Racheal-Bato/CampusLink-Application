using System.ComponentModel.DataAnnotations;

namespace CampusLink_Application.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
