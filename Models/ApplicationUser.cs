using CampusLink_Application.Models;
using Microsoft.AspNetCore.Identity;

namespace CampusLink.Models
{
    // This class extends the default IdentityUser and allows you to store more user info
    public class ApplicationUser : IdentityUser
    {
        // Custom fields you can store in your database
        public string FullName { get; set; }
        public string Gender { get; set; }

        // Optional: Store profile image as a byte array
        public byte[]? ProfileImage { get; set; }

        // Link to Student
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        // Link to Lecturer
        public int? LecturerId { get; set; }
        public Lecturer? Lecturer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
