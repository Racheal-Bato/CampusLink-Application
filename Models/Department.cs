namespace CampusLink_Application.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }

        public List<Student> Students { get; set; }
        public List<Lecturer> Lecturers { get; set; }
    }
}
