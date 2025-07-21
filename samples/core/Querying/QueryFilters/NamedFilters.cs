using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#nullable enable

namespace EFQuerying.QueryFilters;

public static class NamedFilters
{
    public static async Task Sample()
    {
        // First, create the database and add some data to it
        using (var context = new NamedFiltersContext("John"))
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.Blogs.AddRange(
                new() { Name = "John's first blog", TenantId = "John" },
                new() { Name = "John's second blog", TenantId = "John" },
                new() { Name = "Mary's blog", TenantId = "Mary" });
            await context.SaveChangesAsync();
        }

        // Let's delete a blog. Note that although our code seems to delete the blog in the regular way,
        // our override of SaveChangesAsync below will actually modify it instead, setting the IsDeleted property to true.
        using (var context = new NamedFiltersContext("John"))
        {
            var blog = await context.Blogs.FirstAsync(b => b.Name == "John's first blog");
            context.Blogs.Remove(blog);
            await context.SaveChangesAsync();
        }

        // Now, let's query out all blogs.
        // We only get John's second blog, because the first is filtered out by the soft deletion filter, and Mary's blog
        // by the multitenancy filter.
        using (var context = new NamedFiltersContext("John"))
        {
            Console.WriteLine("Blogs:");
            await foreach (var blog in context.Blogs)
            {
                Console.WriteLine(blog.Name);
            }
        }

        // Let's selectively disable only the soft deletion filter:
        using (var context = new NamedFiltersContext("John"))
        {
            Console.WriteLine("Blogs (including soft-deleted ones):");
            #region DisableSoftDeletionFilter
            var allBlogs = await context.Blogs.IgnoreQueryFilters(["SoftDeletionFlter"]).ToListAsync();
            #endregion

            foreach (var blog in allBlogs)
            {
                Console.WriteLine(blog.Name);
            }
        }
    }

    public class NamedFiltersContext(string tenantId) : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Querying.QueryFilters.Blogging;Trusted_Connection=True;ConnectRetryCount=0");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region FilterConfiguration
            modelBuilder.Entity<Blog>()
                .HasQueryFilter("SoftDeletionFlter", b => !b.IsDeleted)
                .HasQueryFilter("TenantFilter", b => b.TenantId == tenantId);
            #endregion
        }

        // The following overrides SaveChangesAsync to add logic which goes over all entities which the user deleted, and changes
        // them to be modified instead, setting the IsDeleted property to true.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();

            foreach (var item in ChangeTracker.Entries<Blog>().Where(e => e.State == EntityState.Deleted))
            {
                item.State = EntityState.Modified;
                item.CurrentValues["IsDeleted"] = true;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
            => throw new NotSupportedException("Use SaveChangesAsync instead.");
    }

    #region Blog
    public class Blog
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public required string TenantId { get; set; }

        public string Name { get; set; }
    }
    #endregion
}
