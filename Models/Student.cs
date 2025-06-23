using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace CampusLink_Application.Models
{
    public class Student
    {
        public int Id { get; set; } // Unique ID (simulate index)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string RegNo { get; set; }
        public DateTime? BirthDate { get; set; }
        public string EmailAdress { get; set; }
        public int? PhoneNumber { get; set; }
        public string ProfilePicturePath { get; set; }

        // This will hold the uploaded file, not stored in the database
        [NotMapped]
        public IFormFile ProfilePicture { get; set; }
    }
}
