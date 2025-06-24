using Microsoft.AspNetCore.Mvc;
using CampusLink_Application.Models;
using CampusLink_Application.Data;
using System.Linq;

namespace CampusLink_Application.Controllers
{
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

        public IActionResult List()
        {
            var students = _context.Students.ToList();
            return View(students);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        public IActionResult Edit(int id)
        {
            var student = _context.Students.Find(id);
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
                student.ProfileImage = updated.ProfileImage;
               
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName); // Adjust as needed

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Save file name to database if needed
                return RedirectToAction("Success");
            }

            return View();
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
