using CampusLink_Application.Data;
using CampusLink_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CampusLink_Application.Controllers
{
    public class LecturerController:Controller
    {
        private readonly AppDbContext _context;

        public LecturerController(AppDbContext context)
        {
            _context = context;
        }
       

        public IActionResult List()
        {
            var lecturers = _context.Lecturers
                .Include(l => l.Department)
                .ToList();

            return View(lecturers);
        }
        

        public IActionResult Register()
        {
            
            ViewBag.Departments = _context.Departments.ToList();
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Register(Lecturer lecturer)
        {
            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }
        

        public IActionResult Edit(int id)
        {
            var lecturer = _context.Lecturers.Find(id);
            if (lecturer == null) return NotFound();


            ViewBag.Departments = _context.Departments.ToList();


            return View(lecturer);
        }
        [HttpPost]
        public IActionResult Edit(Lecturer updated)
        {
            var lecturer = _context.Lecturers.Find(updated.Id);
            if (lecturer != null)
            {


                lecturer.Id = updated.Id;
                lecturer.FirstName = updated.FirstName;
                lecturer.LastName = updated.LastName;
                lecturer.Email = updated.Email;
                lecturer.PhoneNumber = updated.PhoneNumber;
                lecturer.DepartmentId = updated.DepartmentId;


                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
        

        public IActionResult Delete(int id)
        {
            var lecturer = _context.Lecturers.Find(id);
            if (lecturer != null)
            {
                _context.Lecturers.Remove(lecturer);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
