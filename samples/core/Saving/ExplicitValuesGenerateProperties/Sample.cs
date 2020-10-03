using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EFSaving.ExplicitValuesGenerateProperties
{
    public class Sample
    {
        public static async Task RunAsync()
        {
            await using (var context = new EmployeeContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            #region EmploymentStarted
            await using (var context = new EmployeeContext())
            {
                context.Employees.Add(new Employee { Name = "John Doe" });
                context.Employees.Add(new Employee { Name = "Jane Doe", EmploymentStarted = new DateTime(2000, 1, 1) });
                await context.SaveChangesAsync();

                await foreach (var employee in context.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name + ", " + employee.EmploymentStarted);
                }
            }
            #endregion

            await using (var context = new EmployeeContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            #region EmployeeId
            await using (var context = new EmployeeContext())
            {
                context.Employees.Add(new Employee { EmployeeId = 100, Name = "John Doe" });
                context.Employees.Add(new Employee { EmployeeId = 101, Name = "Jane Doe" });

                await context.Database.OpenConnectionAsync();
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Employees ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Employees OFF");
                }
                finally
                {
                    await context.Database.CloseConnectionAsync();
                }

                foreach (var employee in context.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name);
                }
            }
            #endregion

            await using (var context = new EmployeeContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                await context.Database.ExecuteSqlRawAsync(
                    await File.ReadAllTextAsync(@"ExplicitValuesGenerateProperties\employee_UPDATE.sql"));

                context.Employees.Add(new Employee { Name = "John Doe", Salary = 100 });
                context.Employees.Add(new Employee { Name = "Jane Doe", Salary = 100 });
                await context.SaveChangesAsync();
            }

            #region LastPayRaise
            await using (var context = new EmployeeContext())
            {
                var john = context.Employees.Single(e => e.Name == "John Doe");
                john.Salary = 200;

                var jane = context.Employees.Single(e => e.Name == "Jane Doe");
                jane.Salary = 200;
                jane.LastPayRaise = DateTime.Today.AddDays(-7);

                await context.SaveChangesAsync();

                await foreach (var employee in context.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name + ", " + employee.LastPayRaise);
                }
            }
            #endregion
        }
    }
}
