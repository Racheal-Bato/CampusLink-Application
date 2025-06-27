namespace CampusLink_Application.Models
{

    //Join table for many to many (Course-Lecturer)
    public class CourseLecturer
    {
        public int LecturerId { get; set; }
        public Course Course { get; set; }
       

        public int CourseId { get; set; }
        public Lecturer Lecturer { get; set; }
      
    }

}
