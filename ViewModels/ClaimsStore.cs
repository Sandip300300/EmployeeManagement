using System.Security.Claims;

namespace EmployeeManagement.ViewModels
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
            {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role","Edit Role"),
            new Claim("Delete Role","Delete Role")
            };
    }
}
