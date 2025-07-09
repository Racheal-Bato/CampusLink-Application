using CampusLink_Application.Data;
using CampusLink_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList.Extensions;
using X.PagedList;

namespace CampusLink_Application.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        

        public IActionResult List(int? page)
        {
           

            int pageSize = 5; // students per page
            int pageNumber = page ?? 1;

            var students = _context.Students
                .Include(s => s.Course)
                .Include(s => s.Department)
                .OrderBy(s => s.RegNo); // or any sort field

            var pagedStudents = students.ToPagedList(pageNumber, pageSize);
            return View(pagedStudents);
        }
       

        public IActionResult Register()
        {
            ViewBag.Courses = _context.Courses.ToList();
            ViewBag.Departments = _context.Departments.ToList();
            return View();

        }

              [HttpPost]
        public async Task<IActionResult> Register(Student student, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(memoryStream);
                    student.ProfileImage = memoryStream.ToArray(); // Set image to model
                }
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("List");
        }


        public IActionResult Edit(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return NotFound();

            ViewBag.Courses = _context.Courses.ToList();
            ViewBag.Departments = _context.Departments.ToList();

            return View(student);
        }

        [HttpPost]
        public IActionResult Edit(Student updated)
        {
            var student = _context.Students.Find(updated.Id);
            if (student != null)
            {

                student.FirstName = updated.FirstName;
                student.LastName = updated.LastName;
                student.Gender = updated.Gender;
                student.Age = updated.Age;
                student.BirthDate = updated.BirthDate;
                student.PhoneNumber = updated.PhoneNumber;
                student.RegNo = updated.RegNo;
                student.EmailAdress = updated.EmailAdress;
                student.CourseId = updated.CourseId;
                student.DepartmentId = updated.DepartmentId;

                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }


       

        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
