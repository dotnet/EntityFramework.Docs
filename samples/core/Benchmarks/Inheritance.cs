using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

[MemoryDiagnoser]
public class Inheritance
{
    [Params(5000)]
    public int RowsPerEntityType { get; set; }

    [GlobalSetup(Target = nameof(TPH))]
    public async Task SetupTPH()
    {
        Console.WriteLine("Setting up database...");
        using var context = new TPHContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.SeedData(RowsPerEntityType);
        Console.WriteLine("Setup complete.");
    }

    [GlobalSetup(Target = nameof(TPT))]
    public async Task SetupTPT()
    {
        Console.WriteLine("Setting up database...");
        using var context = new TPTContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.SeedData(RowsPerEntityType);
        Console.WriteLine("Setup complete.");
    }

    [GlobalSetup(Target = nameof(TPC))]
    public async Task SetupTPC()
    {
        Console.WriteLine("Setting up database...");
        using var context = new TPCContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.SeedData(RowsPerEntityType);
        Console.WriteLine("Setup complete.");
    }

    [Benchmark]
    public async Task<List<Root>> TPH()
    {
        using var context = new TPHContext();

        return await context.Roots.ToListAsync();
    }

    [Benchmark]
    public async Task<List<Root>> TPT()
    {
        using var context = new TPTContext();

        return await context.Roots.ToListAsync();
    }

    [Benchmark]
    public async Task<List<Root>> TPC()
    {
        using var context = new TPCContext();

        return await context.Roots.ToListAsync();
    }

    public abstract class InheritanceContext : DbContext
    {
        public DbSet<Root> Roots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;ConnectRetryCount=0");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Child1>();
            modelBuilder.Entity<Child1A>();
            modelBuilder.Entity<Child1B>();
            modelBuilder.Entity<Child2>();
            modelBuilder.Entity<Child2A>();
            modelBuilder.Entity<Child2B>();
        }

        public async Task SeedData(int rowsPerEntityType)
        {
            Set<Root>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Root { RootProperty = i }));
            Set<Child1>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child1 { Child1Property = i }));
            Set<Child1A>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child1A { Child1AProperty = i }));
            Set<Child1B>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child1B { Child1BProperty = i }));
            Set<Child2>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child2 { Child2Property = i }));
            Set<Child2A>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child2A { Child2AProperty = i }));
            Set<Child2B>().AddRange(Enumerable.Range(0, rowsPerEntityType).Select(i => new Child2B { Child2BProperty = i }));
            await SaveChangesAsync();
        }
    }

    public class TPHContext : InheritanceContext
    {
    }

    public class TPTContext : InheritanceContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Root>().UseTptMappingStrategy();
        }
    }

    public class TPCContext : InheritanceContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Root>().UseTpcMappingStrategy();
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
