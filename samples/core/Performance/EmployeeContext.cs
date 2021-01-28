using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Performance
{
    public class EmployeeContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True")
                .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
    }
}
