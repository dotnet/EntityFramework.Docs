using Microsoft.Data.Entity;
using System;

namespace EFSaving.ExplicitValuesGenerateProperties
{
    public class Sample
    {
        public static void Run()
        {
            using (var db = new EmployeeContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            using (var db = new EmployeeContext())
            {
                db.Employees.Add(new Employee { Name = "John Doe" });
                db.Employees.Add(new Employee { Name = "Jane Doe", EmploymentStarted = new DateTime(2000, 1, 1) });
                db.SaveChanges();

                foreach (var employee in db.Employees)
                {
                    Console.WriteLine($"{employee.EmployeeId}: {employee.Name}, {employee.EmploymentStarted}");
                }
            }

            using (var db = new EmployeeContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            using (var db = new EmployeeContext())
            {
                db.Employees.Add(new Employee { EmployeeId = 100, Name = "John Doe" });
                db.Employees.Add(new Employee { EmployeeId = 101, Name = "Jane Doe" });

                db.Database.OpenConnection();
                try
                {
                    db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Employee ON");
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Employee OFF");
                }
                finally
                {
                    db.Database.CloseConnection();
                }


                foreach (var employee in db.Employees)
                {
                    Console.WriteLine($"{employee.EmployeeId}: {employee.Name}");
                }
            }
        }
    }
}
