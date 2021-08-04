using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

#nullable enable

public static class ScaffoldNullableReferenceTypesSample
{
    public static void Reverse_engineer_from_database_to_NRTs()
    {
        Console.WriteLine($">>>> Sample: {nameof(Reverse_engineer_from_database_to_NRTs)}");
        Console.WriteLine();

        using var context = new EFCoreSampleContext();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Console.WriteLine();
    }

public partial class Customer
{
    public Customer()
    {
        Orders = new HashSet<Order>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Alias { get; set; }

    public virtual ICollection<Order> Orders { get; set; }
}

public partial class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}

    public partial class EFCoreSampleContext : DbContext
    {
        public EFCoreSampleContext()
        {
        }

        public EFCoreSampleContext(DbContextOptions<EFCoreSampleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EFCoreSample");
            }
        }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

    modelBuilder.Entity<Order>(
        entity =>
        {
            entity.ToTable("Order");

            entity.HasIndex(e => e.CustomerId, "IX_Order_CustomerId");

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId);
        });

    OnModelCreatingPartial(modelBuilder);
}

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
