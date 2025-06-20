using Microsoft.AspNetCore.Mvc;
using CampusLink_Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace CampusLink_Application.Controllers
{
    public class StudentsController : Controller
    {
        static List<Student> students = new List<Student>();
        static int idCounter = 1;

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List()
        {
            return View(students);
        }

        public IActionResult Register()
        {
            return View();
        }
        

        [HttpPost]
        public IActionResult Register(Student student)
        {
            student.Id = idCounter++;//Assigns the student a unique ID
            students.Add(student);
            return RedirectToAction("List");
        }

        public IActionResult Edit(int id)
        {
            var student = students.FirstOrDefault( existingStudents=> existingStudents.Id == id);
            return View(student);
        }

        [HttpPost]
        //controller action method "Edit"
        public IActionResult Edit(Student updatedStudent)
        {
            var student = students.FirstOrDefault(existingStudent => existingStudent.Id == updatedStudent.Id);
            if (student != null)
            {
                student.Name = updatedStudent.Name;
                student.Gender = updatedStudent.Gender;
                student.Age = updatedStudent.Age;
            }
            return RedirectToAction("List");
        }

        public IActionResult Delete(int id)
        {
            var student = students.FirstOrDefault(existingStudents => existingStudents.Id == id);
            if (student != null)
                students.Remove(student);

            return RedirectToAction("List");
        }


    }
}
