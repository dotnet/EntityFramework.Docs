using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class Inheritance
    {
        [Params(5000)]
        public int RowsPerEntityType { get; set; }

        [GlobalSetup(Target = nameof(TPH))]
        public void SetupTPH()
        {
            Console.WriteLine("Setting up database...");
            using var context = new TPHContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SeedData(RowsPerEntityType);
            Console.WriteLine("Setup complete.");
        }

        [GlobalSetup(Target = nameof(TPT))]
        public void SetupTPT()
        {
            Console.WriteLine("Setting up database...");
            using var context = new TPTContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SeedData(RowsPerEntityType);
            Console.WriteLine("Setup complete.");
        }

        [Benchmark]
        public List<Root> TPH()
        {
            using var context = new TPHContext();

            return context.Roots.ToList();
        }

        [Benchmark]
        public List<Root> TPT()
        {
            using var context = new TPTContext();

            return context.Roots.ToList();
        }

        public abstract class InheritanceContext : DbContext
        {
            public DbSet<Root> Roots { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True");

            public void SeedData(int rowsPerEntityType)
            {
                Set<Root>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Root { RootProperty = i }));
                Set<Child1>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child1 { Child1Property = i }));
                Set<Child1A>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child1A { Child1AProperty = i }));
                Set<Child1B>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child1B { Child1BProperty = i }));
                Set<Child2>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child2 { Child2Property = i }));
                Set<Child2A>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child2A { Child2AProperty = i }));
                Set<Child2B>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child2B { Child2BProperty = i }));
                SaveChanges();
            }
        }

        public class TPHContext : InheritanceContext
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Child1>();
                modelBuilder.Entity<Child1A>();
                modelBuilder.Entity<Child1B>();
                modelBuilder.Entity<Child2>();
                modelBuilder.Entity<Child2A>();
                modelBuilder.Entity<Child2B>();
            }
        }

        public class TPTContext : InheritanceContext
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Child1>().ToTable("Child1");
                modelBuilder.Entity<Child1A>().ToTable("Child1A");
                modelBuilder.Entity<Child1B>().ToTable("Child1B");
                modelBuilder.Entity<Child2>().ToTable("Child2");
                modelBuilder.Entity<Child2A>().ToTable("Child2A");
                modelBuilder.Entity<Child2B>().ToTable("Child2B");
            }
        }

        public class Root
        {
            public int Id { get; set; }
            public int RootProperty { get; set; }
        }

        public class Child1 : Root
        {
            public int Child1Property { get; set; }
        }

        public class Child1A : Root
        {
            public int Child1AProperty { get; set; }
        }

        public class Child1B : Root
        {
            public int Child1BProperty { get; set; }
        }

        public class Child2 : Root
        {
            public int Child2Property { get; set; }
        }

        public class Child2A : Root
        {
            public int Child2AProperty { get; set; }
        }

        public class Child2B : Root
        {
            public int Child2BProperty { get; set; }
        }
    }
}
