using System.ComponentModel.DataAnnotations;

namespace CampusLink_Application.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
