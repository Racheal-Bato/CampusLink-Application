using CampusLink.Models;
namespace CampusLink_Application.ViewModels;
public class UserRoleViewModel
{
    public ApplicationUser User { get; set; }
    public IList<string> Roles { get; set; }
}
