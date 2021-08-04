using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public static class OptionalDependentsSample
{
    public static void Handling_optional_dependents_sharing_table_with_principal_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_optional_dependents_sharing_table_with_principal_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

        using (var context = new SomeDbContext())
        {
            var principal = new PrincipalWithOptionalDependents();
            context.Add(principal);

            Console.WriteLine("Saving principal with all null dependent objects:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");

            context.SaveChanges();
        }

        using (var context = new SomeDbContext())
        {
            var principal = context.PrincipalsWithOptionalDependents.Single();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");
        }

        Console.WriteLine();
    }

    public static void Handling_optional_dependents_sharing_table_with_principal_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_optional_dependents_sharing_table_with_principal_2)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

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
            context.SaveChanges();
            Console.WriteLine();
        }

        using (var context = new SomeDbContext())
        {
            var principal = context.PrincipalsWithOptionalDependents.Single();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null. <-- Note dependent is null here.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");
        }

        Console.WriteLine();
    }

    public static void Handling_required_dependents_sharing_table_with_principal()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_required_dependents_sharing_table_with_principal)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

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

            context.SaveChanges();
        }

        using (var context = new SomeDbContext())
        {
            var principal = context.PrincipalsWithRequiredDependents.Single();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOnlyOptionalProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with only required properties is {(principal.DependentWithOnlyRequiredProperties != null ? "not " : "")}null.");
            Console.WriteLine($"  Dependent with both optional and required properties is {(principal.DependentWithOptionalAndRequiredProperties != null ? "not " : "")}null.");
        }

        Console.WriteLine();
    }

    public static void Handling_nested_optional_dependents_sharing_table_with_principal()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_nested_optional_dependents_sharing_table_with_principal)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

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
            context.SaveChanges();
            Console.WriteLine();
        }

        using (var context = new SomeDbContext())
        {
            var principal = context.PrincipalsWithNestedOptionalDependents.Single();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithOptionalNestedDependents != null ? "not " : "")}null.");
            Console.WriteLine($"  Nested dependent with only optional properties is {(principal.DependentWithOptionalNestedDependents?.Nested != null ? "not " : "")}null. <-- Note nested dependent is null here.");
        }

        Console.WriteLine();
    }

    public static void Handling_nested_required_dependents_sharing_table_with_principal()
    {
        Console.WriteLine($">>>> Sample: {nameof(Handling_nested_required_dependents_sharing_table_with_principal)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

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

            context.SaveChanges();
        }

        using (var context = new SomeDbContext())
        {
            var principal = context.PrincipalsWithNestedRequiredDependents.Single();
            Console.WriteLine("After querying back principal and dependents saved above:");
            Console.WriteLine($"  Dependent with only optional properties is {(principal.DependentWithRequiredNestedDependents != null ? "not " : "")}null.");
            Console.WriteLine($"  Nested dependent with only optional properties is {(principal.DependentWithRequiredNestedDependents?.Nested != null ? "not " : "")}null.");
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new SomeDbContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
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

    public class SomeDbContext : DbContext
    {
        public DbSet<PrincipalWithOptionalDependents> PrincipalsWithOptionalDependents { get; set; }
        public DbSet<PrincipalWithRequiredDependents> PrincipalsWithRequiredDependents { get; set; }
        public DbSet<PrincipalWithNestedOptionalDependents> PrincipalsWithNestedOptionalDependents { get; set; }
        public DbSet<PrincipalWithNestedRequiredDependents> PrincipalsWithNestedRequiredDependents { get; set; }

        private readonly bool _quiet;

        public SomeDbContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Warning);
            }
        }
    }
}
