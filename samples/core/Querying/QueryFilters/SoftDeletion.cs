using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

#nullable enable

namespace EFQuerying.QueryFilters;

public static class SoftDeletion
{
    public static async Task Sample()
    {
        // First, create the database and add some data to it
        using (var context = new SoftDeleteContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.Blogs.AddRange(
                new() { Name = "John's blog" },
                new() { Name = "Mary's blog" });
            await context.SaveChangesAsync();
        }

        // Let's delete a blog. Note that although our code seems to delete the blog in the regular way,
        // our override of SaveChangesAsync below will actually modify it instead, setting the IsDeleted property to true.
        using (var context = new SoftDeleteContext())
        {
            var blog = await context.Blogs.FirstAsync(b => b.Name == "John's blog");
            context.Blogs.Remove(blog);
            await context.SaveChangesAsync();
        }

        // Now, let's query out all blogs. The global query filter will ensure that John's blog is not returned, because it has been soft-deleted.
        using (var context = new SoftDeleteContext())
        {
            Console.WriteLine("Blogs:");
            await foreach (var blog in context.Blogs)
            {
                Console.WriteLine(blog.Name);
            }
        }

        // Finally, for auditing reasons, let's now query out all blogs, John's blog is returned even though it has been soft-deleted.
        using (var context = new SoftDeleteContext())
        {
            Console.WriteLine("Blogs (including soft-deleted ones):");
            #region DisableFilter
            var allBlogs = await context.Blogs.IgnoreQueryFilters().ToListAsync();
            #endregion

            foreach (var blog in allBlogs)
            {
                Console.WriteLine(blog.Name);
            }
        }
    }

    public class SoftDeleteContext : DbContext
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
            modelBuilder.Entity<Blog>().HasQueryFilter(b => !b.IsDeleted);
            #endregion
        }

        // The following overrides SaveChangesAsync to add logic which goes over all entities which the user deleted, and changes
        // them to be modified instead, setting the IsDeleted property to true.
        #region SaveChangesAsyncOverride
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
        #endregion

        public override int SaveChanges()
            => throw new NotSupportedException("Use SaveChangesAsync instead.");
    }

    #region Blog
    public class Blog
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }

        public string Name { get; set; }
    }
    #endregion
}
