using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyLine.Data;
using SkyLine.Models;
using System.Diagnostics;
using System.IO;

namespace SkyLine.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet]
        public IActionResult GetIndexView(int deptId, string? search, string sortType,string sortOrder,int pageSize = 20 ,int pageNumber = 1)
        {
            ViewBag.AllDepartments = _context.Departments.ToList();
            ViewBag.SelectedDeptId = deptId;
            ViewBag.CurrentSearch = search;

            IQueryable<Employee> employees = _context.Employees.AsQueryable();

            if (deptId != 0)
            {
                employees = employees.Where(e => e.DepartmentId == deptId);
            }

            if (string.IsNullOrEmpty(search) == false)
            {
                employees = employees.Where(e => e.FullName.Contains(search));
            }

            //sorting 
            if(sortType == "FullName" && sortOrder == "asc")
            {
                employees = employees.OrderBy(e => e.FullName);
            }
            else if( sortType == "FullName" && sortOrder == "desc")
            {
                employees = employees.OrderByDescending(e => e.FullName);
            }
            else if (sortType == "Position" && sortOrder == "asc")
            {
                employees = employees.OrderBy((e) => e.Position);
            }
            else if (sortType == "Position" && sortOrder == "desc")
            {
                employees = employees.OrderByDescending((e) => e.Position);
            }



            //pagination
            if (pageSize > 50 ) pageSize = 50;
            if(pageSize <1) pageSize = 1;
            if (pageNumber <1) pageNumber = 1;

            employees = employees.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            return View("Index", employees);
        }


        [HttpGet]
        public IActionResult GetDetailsView(int id)
        {
            // Eager Loading
            Employee emp = _context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);
            ViewBag.CurrentEmployee = emp;

            if (emp == null)
            {
                return NotFound();
            }
            else
            {
                return View("Details", emp);
            }
        }

        [HttpGet]
        public IActionResult GetCreateView()
        {
            ViewBag.AllDepartments = _context.Departments.ToList();
            return View("Create");
        }

        [HttpPost]
        public IActionResult AddNew(Employee emp, IFormFile? imageFormFile)
        {
            if (((emp.JoinDateTime - emp.BirthDate).Days / 365) < 18)
            {
                ModelState.AddModelError(string.Empty, "Illegal Hiring/Joining Age (Under 18 years old).");
            }

            if (ModelState.IsValid == true)
            {
                if (imageFormFile == null)
                {
                    emp.ImagePath = "\\images\\No_Image_Available.png";
                }
                else
                {
                    Guid imgGuid = Guid.NewGuid(); // sdf54-xym9t-71miw-kjk99-nb12k
                    string imgExtension = Path.GetExtension(imageFormFile.FileName); // .png
                    string imgName = imgGuid + imgExtension; // sdf54-xym9t-71miw-kjk99-nb12k.png
                    emp.ImagePath = "\\images\\employees\\" + imgName;

                    string imgFullPath = _webHostEnvironment.WebRootPath + emp.ImagePath;

                    FileStream fileStream = new FileStream(imgFullPath, FileMode.Create);
                    imageFormFile.CopyTo(fileStream);
                    fileStream.Dispose();
                }

                _context.Employees.Add(emp);
                _context.SaveChanges();

                return RedirectToAction("GetIndexView");
            }
            else
            {
                ViewBag.AllDepartments = _context.Departments.ToList();
                return View("Create", emp);
            }
        }


        [HttpGet]
        public IActionResult GetEditView(int id)
        {
            Employee emp = _context.Employees.Find(id);

            if (emp == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.AllDepartments = _context.Departments.ToList();
                return View("Edit", emp);
            }
        }

        [HttpPost]
        public IActionResult EditCurrent(Employee emp, IFormFile? imageFormFile)
        {
            if (((emp.JoinDateTime - emp.BirthDate).Days / 365) < 18)
            {
                ModelState.AddModelError(string.Empty, "Illegal Hiring/Joining Age (Under 18 years old).");
            }

            if (ModelState.IsValid == true)
            {
                if (imageFormFile != null)
                {
                    if (emp.ImagePath != "\\images\\No_Image_Available.png")
                    {
                        string imgPath = _webHostEnvironment.WebRootPath + emp.ImagePath;
                        if (System.IO.File.Exists(imgPath))
                        {
                            System.IO.File.Delete(imgPath);
                        }
                    }

                    Guid imgGuid = Guid.NewGuid(); // sdf54-xym9t-71miw-kjk99-nb12k
                    string imgExtension = Path.GetExtension(imageFormFile.FileName); // .png
                    string imgName = imgGuid + imgExtension; // sdf54-xym9t-71miw-kjk99-nb12k.png
                    emp.ImagePath = "\\images\\employees\\" + imgName;

                    string imgFullPath = _webHostEnvironment.WebRootPath + emp.ImagePath;

                    FileStream fileStream = new FileStream(imgFullPath, FileMode.Create);
                    imageFormFile.CopyTo(fileStream);
                    fileStream.Dispose();
                }

                _context.Employees.Update(emp);
                _context.SaveChanges();
                return RedirectToAction("GetIndexView");
            }
            else
            {
                ViewBag.AllDepartments = _context.Departments.ToList();
                return View("Edit", emp);
            }
        }


        [HttpGet]
        public IActionResult GetDeleteView(int id)
        {
            Employee emp = _context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);
            ViewBag.CurrentEmployee = emp;

            if (emp == null)
            {
                return NotFound();
            }
            else
            {
                return View("Delete", emp);
            }
        }

        [HttpPost]
        public IActionResult DeleteCurrent(int id)
        {
            Employee emp = _context.Employees.Find(id);

            if (emp.ImagePath != "\\images\\No_Image_Available.png")
            {
                string imgPath = _webHostEnvironment.WebRootPath + emp.ImagePath;
                System.IO.File.Delete(imgPath);
            }

            _context.Employees.Remove(emp);
            _context.SaveChanges();

            return RedirectToAction("GetIndexView");
        }






























        public string GreetVisitor()
        {
            return "Welcome to SkyLine!";
        }

        public string GreetUser(string name)
        {
            return "Hi " + name;
        }






        public string GetAge(string name, int birthYear)
        {
            int ageYears = DateTime.Now.Year - birthYear;
            return $"Hi {name}. You are {ageYears} years old.";
        }


    }
}
