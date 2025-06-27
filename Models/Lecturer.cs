namespace CampusLink_Application.Models
{
    public class Lecturer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? PhoneNumber { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public List<CourseLecturer> CourseLecturers { get; set; } // Many-to-Many with Courses
    }
}
