using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement.ViewModel
{
    public class EmployeeCreateViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Dept Department { get; set; }
        public IFormFile Photo { get; set; }
    }
}
