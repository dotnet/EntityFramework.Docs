using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class ColumnOrderSample
{
    public static void Can_use_ColumnAttribute_to_set_column_order()
    {
        Console.WriteLine($">>>> Sample: {nameof(Can_use_ColumnAttribute_to_set_column_order)}");
        Console.WriteLine();

        using var context = new EmployeeContext();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Console.WriteLine();
    }

    public static class WithOrdering
    {
        #region WithOrdering
        public class EntityBase
        {
            [Column(Order = 1)]
            public int Id { get; set; }

            [Column(Order = 98)]
            public DateTime UpdatedOn { get; set; }

            [Column(Order = 99)]
            public DateTime CreatedOn { get; set; }
        }

        public class PersonBase : EntityBase
        {
            [Column(Order = 2)]
            public string FirstName { get; set; }

            [Column(Order = 3)]
            public string LastName { get; set; }
        }

        public class Employee : PersonBase
        {
            [Column(Order = 20)]
            public string Department { get; set; }

            [Column(Order = 21)]
            public decimal AnnualSalary { get; set; }

            public Address Address { get; set; }
        }

        [Owned]
        public class Address
        {
            [Column("House", Order = 10)]
            public string House { get; set; }

            [Column("Street", Order = 11)]
            public string Street { get; set; }

            [Column("City", Order = 12)]
            public string City { get; set; }

            [Required]
            [Column("Postcode", Order = 13)]
            public string Postcode { get; set; }
        }
        #endregion
    }

    public static class WithoutOrdering
    {
        #region WithoutOrdering
        public class EntityBase
        {
            public int Id { get; set; }
            public DateTime UpdatedOn { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public class PersonBase : EntityBase
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Employee : PersonBase
        {
            public string Department { get; set; }
            public decimal AnnualSalary { get; set; }
            public Address Address { get; set; }
        }

        [Owned]
        public class Address
        {
            public string House { get; set; }
            public string Street { get; set; }
            public string City { get; set; }

            [Required]
            public string Postcode { get; set; }
        }
        #endregion
    }

    public class EmployeeContext : DbContext
    {
        private readonly bool _quiet;

        public EmployeeContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        public DbSet<WithoutOrdering.Employee> EmployeesWithoutOrdering { get; set; }
        public DbSet<WithOrdering.Employee> EmployeesWithOrdering { get; set; }
    }
}
