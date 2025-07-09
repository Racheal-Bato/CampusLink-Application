
namespace CampusLink_Application.Models
{
    public class Course
    {

        public string CourseName { get; set; }
        public int CourseId { set; get; }
        public int? Duration { get; set; }
        public string CourseCode { get; set; }

        public List<Student> Students { get; set; }

        public List<CourseLecturer> CourseLecturers { get; set; } // Many-to-Many with Lecturers

    }
}
