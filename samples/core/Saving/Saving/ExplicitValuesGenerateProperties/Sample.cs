using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace EFSaving.ExplicitValuesGenerateProperties
{
    public class Sample
    {
        public static void Run()
        {
            using (var context = new EmployeeContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            #region EmploymentStarted
            using (var context = new EmployeeContext())
            {
                context.Employees.Add(new Employee { Name = "John Doe" });
                context.Employees.Add(new Employee { Name = "Jane Doe", EmploymentStarted = new DateTime(2000, 1, 1) });
                context.SaveChanges();

                foreach (var employee in context.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name + ", " + employee.EmploymentStarted);
                }
            }
            #endregion

            using (var context = new EmployeeContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            #region EmployeeId
            using (var context = new EmployeeContext())
            {
                context.Employees.Add(new Employee { EmployeeId = 100, Name = "John Doe" });
                context.Employees.Add(new Employee { EmployeeId = 101, Name = "Jane Doe" });

                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Employees ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Employees OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }


                foreach (var employee in context.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name);
                }
            }
            #endregion

            using (var context = new EmployeeContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Database.ExecuteSqlCommand(File.ReadAllText(@"ExplicitValuesGenerateProperties\employee_UPDATE.sql"));

                context.Employees.Add(new Employee { Name = "John Doe", Salary = 100 });
                context.Employees.Add(new Employee { Name = "Jane Doe", Salary = 100 });
                context.SaveChanges();
            }

            #region LastPayRaise
            using (var context = new EmployeeContext())
            {
                var john = context.Employees.Single(e => e.Name == "John Doe");
                john.Salary = 200;

                var jane = context.Employees.Single(e => e.Name == "Jane Doe");
                jane.Salary = 200;
                jane.LastPayRaise = DateTime.Today.AddDays(-7);

                context.SaveChanges();

                foreach (var employee in context.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name + ", " + employee.LastPayRaise);
                }
            }
            #endregion
        }
    }
}
