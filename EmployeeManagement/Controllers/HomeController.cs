using EmployeeManagement.Models;
using EmployeeManagement.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(ILogger<HomeController> logger,IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            HomeDetailsViewModel viewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id),
                PageTitle = "Employee Details"
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                if (vm.Photo != null)
                {
                    var extension = "." + vm.Photo.FileName.Split('.')[vm.Photo.FileName.Split('.').Length - 1];
                    fileName = DateTime.Now.Ticks.ToString() + extension;
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images");

                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }

                    var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", fileName);
                    using (var stream = new FileStream(exactpath, FileMode.Create))
                    {
                        await vm.Photo.CopyToAsync(stream);
                    }
                }
                Employee employee = new Employee
                {
                    
                    Name = vm.Name,
                    Email = vm.Email,
                    Department = vm.Department,
                    PhotoPath = fileName
                };
                _employeeRepository.Add(employee);
                return RedirectToAction("details",new {id = employee.Id});
            }
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            var employee = _employeeRepository.GetEmployee(id);

            var editview = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath,
            };

            return View(editview);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeEditViewModel viewModel)
        {
            if(ModelState.IsValid) 
            {
                Employee employee = _employeeRepository.GetEmployee(viewModel.Id);

                if (employee == null)
                {
                    return NotFound(); // Or handle as appropriate
                }

                employee.Name = viewModel.Name;
                employee.Email = viewModel.Email;
                employee.Department = viewModel.Department;

                if(viewModel.Photo != null)
                {
                    var extension = "." + viewModel.Photo.FileName.Split('.')[viewModel.Photo.FileName.Split('.').Length - 1];
                    var fileName = DateTime.Now.Ticks.ToString() + extension;

                    // Save the photo to the wwwroot/Images directory
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images");
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }

                    var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", fileName);
                    using (var stream = new FileStream(exactpath, FileMode.Create))
                    {
                        await viewModel.Photo.CopyToAsync(stream);
                    }

                    // Update the photo path in the employee object
                    employee.PhotoPath = fileName;
                }
                _employeeRepository.Update(employee);

                // Redirect to details view of the employee
                return RedirectToAction("Index", new { id = employee.Id });

            }
            return View(viewModel);
        }

    }
}
