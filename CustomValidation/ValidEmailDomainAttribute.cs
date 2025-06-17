using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.CustomValidation
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowedDomain;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }

        public override bool IsValid(object value)
        {
            string[] strings = value.ToString().Split('@'); 
            return  strings[1].ToLower() == allowedDomain.ToLower();
        }
    }
}
