using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public static class OptionalDependentsSample
{
    public static async Task Optional_dependents_with_a_required_property()
    {
        Console.WriteLine($">>>> Sample: {nameof(Optional_dependents_with_a_required_property)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using (var context = new SomeDbContext())
        {
            #region NoAddress
            context.Customers1.Add(
                new()
                {
                    Name = "Foul Ole Ron"
                });
            #endregion

            #region PostcodeOnly
            context.Customers1.Add(
                new()
                {
                    Name = "Havelock Vetinari",
                    Address = new()
                    {
                        Postcode = "AN1 1PL",
                    }
                });
            #endregion

            await context.SaveChangesAsync();
        }

        using (var context = new SomeDbContext())
        {
            #region CheckForNullAddress
            await foreach (var customer in context.Customers1.AsAsyncEnumerable())
            {
                Console.Write(customer.Name);

                if (customer.Address == null)
                {
                    Console.WriteLine(" has no address.");
                }
                else
                {
                    Console.WriteLine($" has postcode {customer.Address.Postcode}.");
                }
            }
            #endregion
        }

        Console.WriteLine();
    }

    public static async Task Optional_dependents_without_a_required_property()
    {
        Console.WriteLine($">>>> Sample: {nameof(Optional_dependents_without_a_required_property)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using (var context = new SomeDbContext())
        {
            #region AllNull
            context.Customers2.Add(
                new()
                {
                    Name = "Foul Ole Ron"
                });

            context.Customers2.Add(
                new()
                {
                    Name = "Havelock Vetinari",
                    Address = new()
                });

            #endregion

            await context.SaveChangesAsync();
        }

        Console.WriteLine();

        using (var context = new SomeDbContext())
        {
            var connection = context.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Address_House, Address_Street, Address_City, Address_Postcode FROM Customers2";

            Console.WriteLine($"Id  Name               House   Street  City    Postcode");

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.Write($"{reader.GetInt32(0)}   {reader.GetString(1).PadRight(17)}  ");
                for (int i = 2; i <= 5; i++)
                {
                    Console.Write(reader.IsDBNull(i) ? "NULL    " : reader.GetString(i).PadRight(8));
                }
                Console.WriteLine();
            }

            await connection.CloseAsync();
        }

        Console.WriteLine();
    }

    public static async Task Handling_optional_dependents_sharing_table_with_principal_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_optional_dependents_sharing_table_with_principal_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using (var context = new SomeDbContext())
        {
            var principal = new PrincipalWithOptionalDependents();
            context.Add(principal);

            Console.WriteLine("Saving principal with all null dependent objects:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");

            await context.SaveChangesAsync();
        }

        using (var context = new SomeDbContext())
        {
            var principal = await context.PrincipalsWithOptionalDependents.SingleAsync();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");
        }

        Console.WriteLine();
    }

    public static async Task Handling_optional_dependents_sharing_table_with_principal_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_optional_dependents_sharing_table_with_principal_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using (var context = new SomeDbContext())
        {
            var principal = new PrincipalWithOptionalDependents
            {
                DependentWithOnlyOptionalProperties = new(),
                DependentWithOnlyRequiredProperties = new(),
                DependentWithOptionalAndRequiredProperties = new(),
            };

            context.Add(principal);

            Console.WriteLine("Saving principal with all non-null dependent objects:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");

            Console.WriteLine();
            Console.WriteLine("SaveChanges will warn:");
            Console.WriteLine();
            await context.SaveChangesAsync();
            Console.WriteLine();
        }

        using (var context = new SomeDbContext())
        {
            var principal = await context.PrincipalsWithOptionalDependents.SingleAsync();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null. <-- Note dependent is null here.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");
        }

        Console.WriteLine();
    }

    public static async Task Handling_required_dependents_sharing_table_with_principal()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_required_dependents_sharing_table_with_principal)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using (var context = new SomeDbContext())
        {
            var principal = new PrincipalWithRequiredDependents
            {
                DependentWithOnlyOptionalProperties = new(),
                DependentWithOnlyRequiredProperties = new(),
                DependentWithOptionalAndRequiredProperties = new(),
            };
            context.Add(principal);

            Console.WriteLine("Saving principal with all non-null dependent objects:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");

            await context.SaveChangesAsync();
        }

        using (var context = new SomeDbContext())
        {
            var principal = await context.PrincipalsWithRequiredDependents.SingleAsync();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");
        }

        Console.WriteLine();
    }

    public static async Task Handling_nested_optional_dependents_sharing_table_with_principal()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_nested_optional_dependents_sharing_table_with_principal)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using (var context = new SomeDbContext())
        {
            var principal = new PrincipalWithNestedOptionalDependents()
            {
                DependentWithOptionalNestedDependents = new()
                {
                    Nested = new()
                }
            };

            context.Add(principal);

            Console.WriteLine("Saving principal with all non-null required dependent objects:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOptionalNestedDependents != null ? "not " : "")}null.");
            Console.WriteLine($"  Nested dependent with only optional properties is {(principal.DependentWithOptionalNestedDependents?.Nested != null ? "not " : "")}null.");


            Console.WriteLine();
            Console.WriteLine("SaveChanges will warn:");
            Console.WriteLine();
            await context.SaveChangesAsync();
            Console.WriteLine();
        }

        using (var context = new SomeDbContext())
        {
            var principal = await context.PrincipalsWithNestedOptionalDependents.SingleAsync();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOptionalNestedDependents != null ? "not " : "")}null.");
            Console.WriteLine($"  Nested dependent with only optional properties is {(principal.DependentWithOptionalNestedDependents?.Nested != null ? "not " : "")}null. <-- Note nested dependent is null here.");
        }

        Console.WriteLine();
    }

    public static async Task Handling_nested_required_dependents_sharing_table_with_principal()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_nested_required_dependents_sharing_table_with_principal)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using (var context = new SomeDbContext())
        {
            var principal = new PrincipalWithNestedRequiredDependents()
            {
                DependentWithRequiredNestedDependents = new()
                {
                    Nested = new()
                }
            };

            context.Add(principal);

            Console.WriteLine("Saving principal with all non-null required dependent objects:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithRequiredNestedDependents != null ? "not " : "")}null.");
            Console.WriteLine($"  Nested dependent with only optional properties is {(principal.DependentWithRequiredNestedDependents?.Nested != null ? "not " : "")}null.");

            await context.SaveChangesAsync();
        }

        using (var context = new SomeDbContext())
        {
            var principal = await context.PrincipalsWithNestedRequiredDependents.SingleAsync();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithRequiredNestedDependents != null ? "not " : "")}null.");
            Console.WriteLine($"  Nested dependent with only optional properties is {(principal.DependentWithRequiredNestedDependents?.Nested != null ? "not " : "")}null.");
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new SomeDbContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }

    public class PrincipalWithOptionalDependents
    {
        public int Id { get; set; }

        public DependentWithOptionalAndRequiredProperties DependentWithOptionalAndRequiredProperties { get; set; }
        public DependentWithOnlyOptionalProperties DependentWithOnlyOptionalProperties { get; set; }
        public DependentWithOnlyRequiredProperties DependentWithOnlyRequiredProperties { get; set; }
    }

    public class PrincipalWithRequiredDependents
    {
        public int Id { get; set; }

        public DependentWithOptionalAndRequiredProperties DependentWithOptionalAndRequiredProperties { get; set; }
        public DependentWithOnlyOptionalProperties DependentWithOnlyOptionalProperties { get; set; }
        public DependentWithOnlyRequiredProperties DependentWithOnlyRequiredProperties { get; set; }
    }

    public class DependentWithOptionalAndRequiredProperties
    {
        public int? Optional { get; set; }
        public int Required { get; set; }
    }

    public class DependentWithOnlyOptionalProperties
    {
        public int? Optional { get; set; }
    }

    public class DependentWithOnlyRequiredProperties
    {
        public int Required { get; set; }
    }

    public class PrincipalWithNestedOptionalDependents
    {
        public int Id { get; set; }

        public DependentWithOptionalNestedDependents DependentWithOptionalNestedDependents { get; set; }
    }

    public class DependentWithOptionalNestedDependents
    {
        public int Required { get; set; }
        public DependentWithOnlyOptionalProperties Nested { get; set; }
    }

    public class PrincipalWithNestedRequiredDependents
    {
        public int Id { get; set; }

        public DependentWithRequiredNestedDependents DependentWithRequiredNestedDependents { get; set; }
    }

    public class DependentWithRequiredNestedDependents
    {
        public int? Optional { get; set; }
        public DependentWithOnlyOptionalProperties Nested { get; set; }
    }

    public class WithRequiredProperty
    {
        #region AddressWithRequiredProperty
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Address Address { get; set; }
        }

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

    public class WithoutRequiredProperty
    {
        #region AddressWithoutRequiredProperty
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Address Address { get; set; }
        }

        public class Address
        {
            public string House { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string Postcode { get; set; }
        }
        #endregion
    }

    public class WithRequiredNavigation
    {
        #region AddressWithRequiredNavigation
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }

            [Required]
            public Address Address { get; set; }
        }

        public class Address
        {
            public string House { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string Postcode { get; set; }
        }
        #endregion
    }

    public class WithDifferentTable
    {
        #region AddressWithDifferentTable
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Address Address { get; set; }
        }

        public class Address
        {
            public string House { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string Postcode { get; set; }
        }
        #endregion
    }

    public class NestedWithoutRequiredProperty
    {
        #region NestedWithoutRequiredProperty
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ContactInfo ContactInfo { get; set; }
        }

        public class ContactInfo
        {
            public string Phone { get; set; }
            public Address Address { get; set; }
        }

        public class Address
        {
            public string House { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string Postcode { get; set; }
        }
        #endregion
    }

    public class SomeDbContext : DbContext
    {
        public DbSet<PrincipalWithOptionalDependents> PrincipalsWithOptionalDependents { get; set; }
        public DbSet<PrincipalWithRequiredDependents> PrincipalsWithRequiredDependents { get; set; }
        public DbSet<PrincipalWithNestedOptionalDependents> PrincipalsWithNestedOptionalDependents { get; set; }
        public DbSet<PrincipalWithNestedRequiredDependents> PrincipalsWithNestedRequiredDependents { get; set; }

        public DbSet<WithRequiredProperty.Customer> Customers1 { get; set; }
        public DbSet<WithoutRequiredProperty.Customer> Customers2 { get; set; }

        private readonly bool _quiet;

        public SomeDbContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<WithRequiredProperty.Customer>()
                .OwnsOne(e => e.Address);

            modelBuilder
                .Entity<WithoutRequiredProperty.Customer>()
                .OwnsOne(e => e.Address);

            #region RequiredInModel
            modelBuilder.Entity<WithRequiredNavigation.Customer>(
                b =>
                    {
                        b.OwnsOne(e => e.Address);
                        b.Navigation(e => e.Address).IsRequired();
                    });
            #endregion

            #region WithDifferentTable
            modelBuilder
                .Entity<WithDifferentTable.Customer>(
                    b =>
                        {
                            b.ToTable("Customers");
                            b.OwnsOne(
                                e => e.Address,
                                b => b.ToTable("CustomerAddresses"));
                        });
            #endregion

            modelBuilder.Entity<PrincipalWithOptionalDependents>(
                b =>
                {
                    b.OwnsOne(e => e.DependentWithOptionalAndRequiredProperties);
                    b.OwnsOne(e => e.DependentWithOnlyOptionalProperties);
                    b.OwnsOne(e => e.DependentWithOnlyRequiredProperties);
                });

            modelBuilder.Entity<PrincipalWithRequiredDependents>(
                b =>
                {
                    b.OwnsOne(e => e.DependentWithOptionalAndRequiredProperties);
                    b.Navigation(e => e.DependentWithOptionalAndRequiredProperties).IsRequired();
                    b.OwnsOne(e => e.DependentWithOnlyOptionalProperties);
                    b.Navigation(e => e.DependentWithOnlyOptionalProperties).IsRequired();
                    b.OwnsOne(e => e.DependentWithOnlyRequiredProperties);
                    b.Navigation(e => e.DependentWithOnlyRequiredProperties).IsRequired();
                });

            modelBuilder.Entity<PrincipalWithNestedOptionalDependents>(
                b =>
                {
                    b.OwnsOne(e => e.DependentWithOptionalNestedDependents, b =>
                    {
                        b.OwnsOne(e => e.Nested);
                    });
                });

            modelBuilder.Entity<PrincipalWithNestedRequiredDependents>(
                b =>
                {
                    b.OwnsOne(e => e.DependentWithRequiredNestedDependents, b =>
                    {
                        b.OwnsOne(e => e.Nested);
                        b.Navigation(e => e.Nested).IsRequired();
                    });
                    b.Navigation(e => e.DependentWithRequiredNestedDependents).IsRequired();
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Warning);
            }
        }
    }
}
