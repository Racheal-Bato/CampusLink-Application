using CampusLink_Application.Data;
using CampusLink_Application.Models;
using Microsoft.AspNetCore.Mvc;


namespace CampusLink_Application.Controllers
{
    public class DepartmentController:Controller
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult List()
        {
            var departments = _context.Departments.ToList();

            return View(departments);
        }
        public IActionResult Register()
        {
           
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Register(Department department)
        {


            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return RedirectToAction("List");
        }
        public IActionResult Edit(int id)
        {
            var department = _context.Departments.Find(id);
            if (department == null) 
                return NotFound();

            
       

            return View(department);
        }
        [HttpPost]
        public IActionResult Edit(Department updated)
        {
            var department = _context.Departments.Find(updated.Id);
            if (department != null)
            {


                department.Id = updated.Id;
                department.DepartmentName = updated.DepartmentName;

                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
        public IActionResult Delete(int id)
        {
            var department = _context.Departments.Find(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
