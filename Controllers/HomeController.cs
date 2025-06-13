using System.Diagnostics;
using EmployeeManagement.Models;
using EmployeeManagement.Repository;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

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
