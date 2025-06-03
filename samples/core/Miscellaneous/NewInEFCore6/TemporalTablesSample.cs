using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class TemporalTablesSample
{
    public static async Task Use_SQL_Server_temporal_tables()
    {
        Console.WriteLine($">>>> Sample: {nameof(Use_SQL_Server_temporal_tables)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        DateTime timeStamp1;
        DateTime timeStamp2;
        DateTime timeStamp3;
        DateTime timeStamp4;

        using (var context = new EmployeeContext(quiet: true))
        {
            #region InsertData
            context.AddRange(
                new Employee
                {
                    Name = "Pinky Pie",
                    Address = "Sugarcube Corner, Ponyville, Equestria",
                    Department = "DevDiv",
                    Position = "Party Organizer",
                    AnnualSalary = 100.0m
                },
                new Employee
                {
                    Name = "Rainbow Dash",
                    Address = "Cloudominium, Ponyville, Equestria",
                    Department = "DevDiv",
                    Position = "Ponyville weather patrol",
                    AnnualSalary = 900.0m
                },
                new Employee
                {
                    Name = "Fluttershy",
                    Address = "Everfree Forest, Equestria",
                    Department = "DevDiv",
                    Position = "Animal caretaker",
                    AnnualSalary = 30.0m
                });

            await context.SaveChangesAsync();
            #endregion
        }

        using (var context = new EmployeeContext(quiet: true))
        {
            Console.WriteLine();
            Console.WriteLine("Starting data:");

            var employees = await context.Employees.ToListAsync();
            foreach (var employee in employees)
            {
                var employeeEntry = context.Entry(employee);
                var validFrom = employeeEntry.Property<DateTime>("ValidFrom").CurrentValue;
                var validTo = employeeEntry.Property<DateTime>("ValidTo").CurrentValue;

                Console.WriteLine($"  Employee {employee.Name} valid from {validFrom} to {validTo}");
            }
        }

        using (var context = new EmployeeContext(quiet: true))
        {
            // Change the sleep values to emphasize the temporal nature of the data.
            const int millisecondsDelay = 100;

            Thread.Sleep(millisecondsDelay);
            timeStamp1 = DateTime.UtcNow;
            Thread.Sleep(millisecondsDelay);

            var employee = await context.Employees.SingleAsync(e => e.Name == "Rainbow Dash");
            employee.Position = "Wonderbolt Trainee";
            await context.SaveChangesAsync();

            Thread.Sleep(millisecondsDelay);
            timeStamp2 = DateTime.UtcNow;
            Thread.Sleep(millisecondsDelay);

            employee.Position = "Wonderbolt Reservist";
            await context.SaveChangesAsync();

            Thread.Sleep(millisecondsDelay);
            timeStamp3 = DateTime.UtcNow;
            Thread.Sleep(millisecondsDelay);

            employee.Position = "Wonderbolt";
            await context.SaveChangesAsync();

            Thread.Sleep(millisecondsDelay);
            timeStamp4 = DateTime.UtcNow;
            Thread.Sleep(millisecondsDelay);
        }

        using (var context = new EmployeeContext(quiet: true))
        {
            #region NormalQuery
            var employee = await context.Employees.SingleAsync(e => e.Name == "Rainbow Dash");
            context.Remove(employee);
            await context.SaveChangesAsync();
            #endregion
        }

        using (var context = new EmployeeContext(quiet: true))
        {
            Console.WriteLine();
            Console.WriteLine("After updates and delete:");

            #region TrackingQuery
            var employees = await context.Employees.ToListAsync();
            foreach (var employee in employees)
            {
                var employeeEntry = context.Entry(employee);
                var validFrom = employeeEntry.Property<DateTime>("ValidFrom").CurrentValue;
                var validTo = employeeEntry.Property<DateTime>("ValidTo").CurrentValue;

                Console.WriteLine($"  Employee {employee.Name} valid from {validFrom} to {validTo}");
            }
            #endregion

            Console.WriteLine();
            Console.WriteLine("Historical data for Rainbow Dash:");

            #region TemporalAll
            var history = await context
                .Employees
                .TemporalAll()
                .Where(e => e.Name == "Rainbow Dash")
                .OrderBy(e => EF.Property<DateTime>(e, "ValidFrom"))
                .Select(
                    e => new
                    {
                        Employee = e,
                        ValidFrom = EF.Property<DateTime>(e, "ValidFrom"),
                        ValidTo = EF.Property<DateTime>(e, "ValidTo")
                    })
                .ToListAsync();

            foreach (var pointInTime in history)
            {
                Console.WriteLine(
                    $"  Employee {pointInTime.Employee.Name} was '{pointInTime.Employee.Position}' from {pointInTime.ValidFrom} to {pointInTime.ValidTo}");
            }
            #endregion
        }

        using (var context = new EmployeeContext(quiet: true))
        {
            Console.WriteLine();
            Console.WriteLine($"Historical data for Rainbow Dash between {timeStamp2} and {timeStamp3}:");

            #region TemporalBetween
            var history = await context
                .Employees
                .TemporalBetween(timeStamp2, timeStamp3)
                .Where(e => e.Name == "Rainbow Dash")
                .OrderBy(e => EF.Property<DateTime>(e, "ValidFrom"))
                .Select(
                    e => new
                    {
                        Employee = e,
                        ValidFrom = EF.Property<DateTime>(e, "ValidFrom"),
                        ValidTo = EF.Property<DateTime>(e, "ValidTo")
                    })
                .ToListAsync();
            #endregion

            foreach (var pointInTime in history)
            {
                Console.WriteLine(
                    $"  Employee {pointInTime.Employee.Name} was '{pointInTime.Employee.Position}' from {pointInTime.ValidFrom} to {pointInTime.ValidTo}");
            }
        }

        using (var context = new EmployeeContext(quiet: true))
        {
            Console.WriteLine();
            Console.WriteLine($"Historical data for Rainbow Dash as of {timeStamp2}:");

            var history = await context
                .Employees
                .TemporalAsOf(timeStamp2)
                .Where(e => e.Name == "Rainbow Dash")
                .OrderBy(e => EF.Property<DateTime>(e, "ValidFrom"))
                .Select(
                    e => new
                    {
                        Employee = e,
                        ValidFrom = EF.Property<DateTime>(e, "ValidFrom"),
                        ValidTo = EF.Property<DateTime>(e, "ValidTo")
                    })
                .ToListAsync();

            foreach (var pointInTime in history)
            {
                Console.WriteLine(
                    $"  Employee {pointInTime.Employee.Name} was '{pointInTime.Employee.Position}' from {pointInTime.ValidFrom} to {pointInTime.ValidTo}");
            }
        }

        using (var context = new EmployeeContext(quiet: true))
        {
            Console.WriteLine();
            Console.WriteLine($"Restoring Rainbow Dash from {timeStamp2}...");

            #region RestoreData
            var employee = await context
                .Employees
                .TemporalAsOf(timeStamp2)
                .SingleAsync(e => e.Name == "Rainbow Dash");

            context.Add(employee);
            await context.SaveChangesAsync();
            #endregion

            Console.WriteLine();
            Console.WriteLine($"Historical data for Rainbow Dash between:");

            var history = await context
                .Employees
                .TemporalAll()
                .Where(e => e.Name == "Rainbow Dash")
                .OrderBy(e => EF.Property<DateTime>(e, "ValidFrom"))
                .Select(
                    e => new
                    {
                        Employee = e,
                        ValidFrom = EF.Property<DateTime>(e, "ValidFrom"),
                        ValidTo = EF.Property<DateTime>(e, "ValidTo")
                    })
                .ToListAsync();

            foreach (var pointInTime in history)
            {
                Console.WriteLine(
                    $"  Employee {pointInTime.Employee.Name} was '{pointInTime.Employee.Position}' from {pointInTime.ValidFrom} to {pointInTime.ValidTo}");
            }
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using (var context = new EmployeeContext(quiet: true))
            {
                await context.Database.EnsureDeletedAsync();
            }

            using (var context = new EmployeeContext())
            {
                await context.Database.EnsureCreatedAsync();
            }
        }
    }

    #region Employee
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Address { get; set; }
        public decimal AnnualSalary { get; set; }
    }
    #endregion

    public class EmployeeContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        private readonly bool _quiet;

        public EmployeeContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region SimpleConfig
            modelBuilder
                .Entity<Employee>()
                .ToTable("Employees", b => b.IsTemporal());
            #endregion

            #region AdvancedConfig
            modelBuilder
                .Entity<Employee>()
                .ToTable(
                    "Employees",
                    b => b.IsTemporal(
                        b =>
                        {
                            b.HasPeriodStart("ValidFrom");
                            b.HasPeriodEnd("ValidTo");
                            b.UseHistoryTable("EmployeeHistoricalData");
                        }));
            #endregion

            modelBuilder
                .Entity<Employee>(
                    b =>
                    {
                        b.Property(e => e.Name).HasMaxLength(100);
                        b.Property(e => e.Position).HasMaxLength(100);
                        b.Property(e => e.Department).HasMaxLength(100);
                        b.Property(e => e.Address).HasMaxLength(1024);
                        b.Property(e => e.AnnualSalary).HasPrecision(10, 2);
                    });
        }
    }
}
