#nullable enable
using System.ComponentModel.DataAnnotations;

namespace CampusLink_Application.ViewModels
{
    public class Verify2FAVm
    {
        [Required]
        [Display(Name = "Security Code")]
        public string Code { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}
