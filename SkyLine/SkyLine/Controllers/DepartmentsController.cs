using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyLine.Data;
using SkyLine.Models;

namespace SkyLine.Controllers
{

    public class DepartmentsController : Controller
    {
        ApplicationDbContext _context;
        IWebHostEnvironment _webHostEnvironment;

        public DepartmentsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetIndexView(string? search,string sortType, string sortOrder, int pageSize = 20, int pageNumber = 1)
        {

            ViewBag.CurrentSearch = search;

            IQueryable<Department> departments = _context.Departments.AsQueryable();

            if (string.IsNullOrEmpty(search) == false)
            {
                departments = departments.Where(d => d.Name.Contains(search));
            }


            //sorting 
            if (sortType == "Name" && sortOrder == "asc")
            {
                departments = departments.OrderBy(d => d.Name);
            }
            else if (sortType == "FullName" && sortOrder == "desc")
            {
                departments = departments.OrderByDescending(d=> d.Name);
            }
            else if (sortType == "Description" && sortOrder == "asc")
            {
                departments = departments.OrderBy(d => d.Description);
            }
            else if (sortType == "Description" && sortOrder == "desc")
            {
                departments = departments.OrderByDescending((d) => d.Description);
            }

            //pagination
            if (pageSize > 50) pageSize = 50;
            if (pageSize < 1) pageSize = 1;
            if (pageNumber < 1) pageNumber = 1;

            departments = departments.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            // return View("Index", _context.Departments.ToList());
            return View("Index", departments);

        }



        [HttpGet]
        public IActionResult GetDetailsView(int id)
        {
            Department dept = _context.Departments.Include(d=>d.Employees).FirstOrDefault(d => d.Id == id);
            ViewBag.CurrentDepartment = dept;

            ViewBag.CurrentDept = dept;

            if (dept == null)
            {
                return NotFound();
            }
            else
            {
                return View("Details", dept);
            }
        }

        [HttpGet]
        public IActionResult GetCreateView()
        {
            return View("Create");
        }


        // HTTPVerbs -> HTTPGET - HTTPPOST
        [HttpPost]
        public IActionResult AddNew(Department dept, IFormFile? imageFormFile)
        {
            if (dept.StartDate < new DateTime(2014, 2, 1))
            {
                ModelState.AddModelError(string.Empty, "Start Date must be after 31 January 2014");
            }

            if (ModelState.IsValid == true)
            {
                _context.Departments.Add(dept);
                _context.SaveChanges();

                return RedirectToAction("GetIndexView");
            }
            else
            {
                return View("Create", dept);
            }
        }


        [HttpGet]
        public IActionResult GetEditView(int id)
        {
            Department department = _context.Departments.FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }
            else
            {
                return View("Edit", department);
            }
        }

        [HttpPost]
        public IActionResult EditCurrent(Department dept, IFormFile? imageFormFile)
        {
            if (dept.StartDate < new DateTime(2014, 2, 1))
            {
                ModelState.AddModelError(string.Empty, "Start Date must be after 31 January 2014");
            }

            if (ModelState.IsValid == true)
            {
                _context.Departments.Update(dept);
                _context.SaveChanges();

                return RedirectToAction("GetIndexView");
            }
            else
            {
                return View("Edit", dept);
            }
        }

        [HttpGet]
        public IActionResult GetDeleteView(int id)
        {
            Department dept = _context.Departments.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
            ViewBag.CurrentDepartment = dept;

            ViewBag.CurrentDept = dept;

            if (dept == null)
            {
                return NotFound();
            }
            else
            {
                return View("Delete", dept);
            }
        }

        [HttpPost]
        public IActionResult DeleteCurrent(int id)
        {
            Department department = _context.Departments.Find(id);

            _context.Departments.Remove(department);
            _context.SaveChanges();

            return RedirectToAction("GetIndexView");
        }























        // https://localhost:7190/Departments/GreetVisitor
        public string GreetVisitor(string deptName)
        {
            return "Welcome to " + deptName + " department!";
        }

        // https://localhost:7190/Departments/GreetUser?firstName=Folan&lastName=Alfolani
        public string GreetUser(string firstName, string lastName)
        {
            return $"Hi {firstName} {lastName}!";
        }

        public string GetAge(int birthYear)
        {
            return (DateTime.Now.Year - birthYear).ToString();
        }

        public string IsLegalHiringAge(int birthYear)
        {
            return (DateTime.Now.Year - birthYear >= 18).ToString();
        }
    }


}
