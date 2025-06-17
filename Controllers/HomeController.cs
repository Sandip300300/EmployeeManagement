using EmployeeManagement.Models;
using EmployeeManagement.Repository;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IEmployeeRepository _employeeRepository;

        public HomeController(ILogger<HomeController> logger, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        [AllowAnonymous]
        public ViewResult Details(int id)
        {
            var viewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id),
                PageTitle = "Employee Details"
            };
            return  View(viewModel);
        }

        public ViewResult Create()
        {
           return View();
        }

        [HttpPost]
        public RedirectToActionResult Create(CreateViewModel viewModel)
        {
            var employee = new Employee()
            {
                Name = viewModel.Name,
                Email = viewModel.Email,
                Department = viewModel.Department
            };
            if (viewModel == null)
            {
                ModelState.AddModelError("", "Invalid employee data.");
                return RedirectToAction("Create");
            }
            if (viewModel.Photo != null && viewModel.Photo.Length > 0)
            {
                var fileName = Path.GetFileName(viewModel.Photo.FileName);
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    viewModel.Photo.CopyTo(fileStream);
                } 
                employee.PhotoPath = fileName;
            }
            Employee newEmployee = _employeeRepository.Add(employee);
            return RedirectToAction("details", new { id = newEmployee.Id });
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                if (employee == null)
                {
                    return NotFound();
                }

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != null && model.Photo.Length > 0)
                {
                    if (!string.IsNullOrEmpty(model.ExistingPhotoPath))
                    {
                        var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", model.ExistingPhotoPath);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }

                    var fileName = Path.GetFileName(model.Photo.FileName);
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
                    var filePath = Path.Combine(uploads, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Photo.CopyTo(fileStream);
                    }

                    employee.PhotoPath = fileName;
                }

                _employeeRepository.Update(employee); // Assuming Update() exists in your repository
                return RedirectToAction("details", new { id = employee.Id });
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var employee = _employeeRepository.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Delete photo from wwwroot/images
            if (!string.IsNullOrEmpty(employee.PhotoPath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", employee.PhotoPath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _employeeRepository.Delete(id); // Make sure this method exists in your repository
            return RedirectToAction("index"); // Redirect to landing/list page
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
