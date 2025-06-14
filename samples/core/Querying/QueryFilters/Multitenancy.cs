using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

#nullable enable

namespace EFQuerying.QueryFilters;

public static class Multitenancy
{
    public static async Task Sample()
    {
        // First, create the database and add some data to it
        using (var context = new MultitenancyContext("John"))
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.Blogs.AddRange(
                new() { Name = "John's blog", TenantId = "John" },
                new() { Name = "Mary's blog", TenantId = "Mary" });
            await context.SaveChangesAsync();
        }

        // Now, let's query out all blogs.
        // Since we specify Mary as the tenant ID for the context, only Mary's blogs will be returned.
        using (var context = new MultitenancyContext("Mary"))
        {
            Console.WriteLine("Blogs:");
            await foreach (var blog in context.Blogs)
            {
                Console.WriteLine(blog.Name);
            }
        }
    }

    public class MultitenancyContext(string tenantId) : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Querying.QueryFilters.Blogging;Trusted_Connection=True;ConnectRetryCount=0");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasQueryFilter(b => b.TenantId == tenantId);
        }
    }

    #region Blog
    public class Blog
    {
        public int Id { get; set; }
        public required string TenantId { get; set; }

        public string Name { get; set; }
    }
    #endregion
}
