using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach(var foreignkey in modelBuilder.Model.GetEntityTypes().SelectMany(e=>e.GetForeignKeys()))
            {
                foreignkey.DeleteBehavior = DeleteBehavior.Restrict;
            }
            //modelBuilder.Entity<Employee>().HasData(
            //    new Employee
            //    {
            //        Id = 2,
            //        Name = "Mary",
            //        Department = Dept.HR,
            //        Email="Mary@gmail.com"

            //    }
            //    ,
            //    new Employee
            //    {
            //        Id = 3,
            //        Name = "Jhon",
            //        Department = Dept.PayRoll,
            //        Email = "Jhon@gmail.com"

            //    }

            //    );
            ;
        }
    }
}
