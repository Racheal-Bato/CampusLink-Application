using CampusLink_Application.Data;
using CampusLink_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusLink_Application.Controllers
{
    public class CourseController:Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult List()
        {

            var courses = _context.Courses.ToList();
            return View(courses);
        }
        public IActionResult Register()
        {
            ViewBag.Courses = _context.Courses.ToList();
            //ViewBag.Departments = _context.Departments.ToList();
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Register(Course course)
        {
           

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return RedirectToAction("List");
        }
        public IActionResult Edit(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();

            ViewBag.Courses = _context.Courses.ToList();
            ViewBag.Departments = _context.Departments.ToList();

            return View(course);
        }
        [HttpPost]
        public IActionResult Edit(Course updated)
        {
            var course = _context.Courses.Find(updated.CourseId);
            if (course != null)
            {

                
                course.CourseId = updated.CourseId;
                course.CourseName = updated.CourseName;
                course.Duration = updated.Duration;
                course.CourseCode = updated.CourseCode;
                course.CourseLecturers = updated.CourseLecturers;
                course.Students = updated.Students;

                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }

    }
}
