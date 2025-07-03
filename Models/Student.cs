using CampusLink.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



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
        public byte[]? ProfileImage { get; set; }

        public int CourseId { get; set; }         // One-to-Many
        public Course Course { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]// One-to-Many
        public Department Department { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;// 👈 Add this

        public string? RegistrationNumber { get; set; } // 👈 For Student
      
    }


}
    
   




