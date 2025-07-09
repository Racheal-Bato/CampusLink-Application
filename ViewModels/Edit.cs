using System.ComponentModel.DataAnnotations;

namespace CampusLink_Application.ViewModels
{
    public class Edit
    {
        public string Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
