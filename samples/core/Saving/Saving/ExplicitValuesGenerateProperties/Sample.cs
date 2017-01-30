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
            using (var db = new EmployeeContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            #region EmploymentStarted
            using (var db = new EmployeeContext())
            {
                db.Employees.Add(new Employee { Name = "John Doe" });
                db.Employees.Add(new Employee { Name = "Jane Doe", EmploymentStarted = new DateTime(2000, 1, 1) });
                db.SaveChanges();

                foreach (var employee in db.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name + ", " + employee.EmploymentStarted);
                }
            }
            #endregion

            using (var db = new EmployeeContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            #region EmployeeId
            using (var db = new EmployeeContext())
            {
                db.Employees.Add(new Employee { EmployeeId = 100, Name = "John Doe" });
                db.Employees.Add(new Employee { EmployeeId = 101, Name = "Jane Doe" });

                db.Database.OpenConnection();
                try
                {
                    db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Employees ON");
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Employees OFF");
                }
                finally
                {
                    db.Database.CloseConnection();
                }


                foreach (var employee in db.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name);
                }
            }
            #endregion

            using (var db = new EmployeeContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Database.ExecuteSqlCommand(File.ReadAllText(@"ExplicitValuesGenerateProperties\employee_UPDATE.sql"));

                db.Employees.Add(new Employee { Name = "John Doe", Salary = 100 });
                db.Employees.Add(new Employee { Name = "Jane Doe", Salary = 100 });
                db.SaveChanges();
            }

            #region LastPayRaise
            using (var db = new EmployeeContext())
            {
                var john = db.Employees.Single(e => e.Name == "John Doe");
                john.Salary = 200;

                var jane = db.Employees.Single(e => e.Name == "Jane Doe");
                jane.Salary = 200;
                jane.LastPayRaise = DateTime.Today.AddDays(-7);

                db.SaveChanges();

                foreach (var employee in db.Employees)
                {
                    Console.WriteLine(employee.EmployeeId + ": " + employee.Name + ", " + employee.LastPayRaise);
                }
            }
            #endregion
        }
    }
}
