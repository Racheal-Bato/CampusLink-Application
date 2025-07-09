using System.Collections.Generic;

namespace CampusLink_Application.ViewModels
{
    public class AssignRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>();
    }
}
