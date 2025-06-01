using EmployeeManagement.Models;

namespace EmployeeManagement.ViewModels
{
    public class CreateViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Dept? Department { get; set; }
    }
}
