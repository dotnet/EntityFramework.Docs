using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.QueryFilters;

public class BloggingContext : DbContext
{
    private readonly string _tenantId;

    public BloggingContext(string tenant)
    {
        _tenantId = tenant;
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Querying.QueryFilters.Blogging;Trusted_Connection=True;ConnectRetryCount=0");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().Property<string>("_tenantId").HasColumnName("TenantId");

        // Configure entity filters
        #region FilterConfiguration
        modelBuilder.Entity<Blog>().HasQueryFilter(b => EF.Property<string>(b, "_tenantId") == _tenantId);
        modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
        #endregion
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();

        foreach (var item in ChangeTracker.Entries().Where(
                     e =>
                         e.State == EntityState.Added && e.Metadata.GetProperties().Any(p => p.Name == "_tenantId")))
        {
            item.CurrentValues["_tenantId"] = _tenantId;
        }

        foreach (var item in ChangeTracker.Entries<Post>().Where(e => e.State == EntityState.Deleted))
        {
            item.State = EntityState.Modified;
            item.CurrentValues["IsDeleted"] = true;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
