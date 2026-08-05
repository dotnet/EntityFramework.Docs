using Microsoft.EntityFrameworkCore;

namespace EFModeling.ComplexTypes;

public class ComplexTypesContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Distributor> Distributors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFComplexTypes;Trusted_Connection=True;ConnectRetryCount=0");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region ComplexPropertyConfig
        modelBuilder.Entity<Customer>(b =>
        {
            // A required complex property; mapped to columns in the Customers table.
            b.ComplexProperty(c => c.Address);

            // An optional complex property (EF Core 10+).
            b.ComplexProperty(c => c.SecondaryAddress);
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.ComplexProperty(o => o.ShippingAddress);
            b.ComplexProperty(o => o.BillingAddress);
        });
        #endregion

        #region ComplexPropertyFacets
        modelBuilder.Entity<Order>()
            .ComplexProperty(
                o => o.ShippingAddress,
                b =>
                {
                    b.Property(a => a.Line1).HasColumnName("ShipsToStreet").HasMaxLength(100);
                    b.Property(a => a.City).HasColumnName("ShipsToCity");
                });
        #endregion

        #region ComplexCollectionJson
        // Collections of complex types must be mapped to JSON on relational providers.
        modelBuilder.Entity<Distributor>()
            .ComplexCollection(d => d.ShippingCenters, b => b.ToJson());
        #endregion

        #region PropertyChaining
        // EF Core 11 allows configuring a complex-type property directly by chaining
        // member access, without first obtaining the complex-type builder.
        modelBuilder.Entity<Customer>()
            .Property(c => c.Address.Line1)
            .HasMaxLength(200);
        #endregion

        #region IndexOnComplexProperty
        // EF Core 11 allows keys and indexes to target scalar properties nested
        // inside non-collection complex types.
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Address.PostCode);
        #endregion
    }
}

// Indexing into a JSON-mapped complex collection requires a database that supports
// JSON indexes (for example, SQL Server 2025). This context hosts that example
// separately so the main sample can run against LocalDB.
public class ComplexTypesJsonIndexContext : DbContext
{
    public DbSet<Distributor> Distributors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFComplexTypesJsonIndex;Trusted_Connection=True;ConnectRetryCount=0");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Distributor>()
            .ComplexCollection(d => d.ShippingCenters, b => b.ToJson());

        #region IndexOnComplexCollection
        // Index a scalar inside every element of a JSON-mapped complex collection.
        // Requires a provider/database with JSON index support, such as SQL Server 2025.
        modelBuilder.Entity<Distributor>()
            .HasIndex("ShippingCenters[].City");
        #endregion
    }
}

public class ComplexTypesDiscriminatorContext : DbContext
{
    public DbSet<Place> Places { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFComplexTypesDiscriminator;Trusted_Connection=True;ConnectRetryCount=0");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region OptionalWithDiscriminator
        // GeoLocation has no required property, so configure a discriminator. EF creates
        // it as a required shadow property, which satisfies the requirement that an
        // optional complex type have at least one required property.
        modelBuilder.Entity<Place>()
            .ComplexProperty(p => p.Location, b => b.HasDiscriminator());
        #endregion
    }
}

