using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.QueryFilters;

public static class QueryFiltersAndRequiredNavigations
{
    public static async Task Sample()
    {
        using (var db = new QueryFiltersAndRequiredNavigationsContext())
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            #region SeedData
            db.Blogs.Add(
                new Blog
                {
                    Url = "http://sample.com/blogs/fish",
                    Posts =
                    [
                        new() { Title = "Fish care 101" },
                        new() { Title = "Caring for tropical fish" },
                        new() { Title = "Types of ornamental fish" }
                    ]
                });

            db.Blogs.Add(
                new Blog
                {
                    Url = "http://sample.com/blogs/cats",
                    Posts =
                    [
                        new() { Title = "Cat care 101" },
                        new() { Title = "Caring for tropical cats" },
                        new() { Title = "Types of ornamental cats" }
                    ]
                });
            #endregion

            await db.SaveChangesAsync();
        }

        Console.WriteLine("Use of required navigations to access entity with query filter demo");
        using (var db = new QueryFiltersAndRequiredNavigationsContext())
        {
            #region Queries
            var allPosts = await db.Posts.ToListAsync();
            var allPostsWithBlogsIncluded = await db.Posts.Include(p => p.Blog).ToListAsync();
            #endregion

            if (allPosts.Count == allPostsWithBlogsIncluded.Count)
            {
                Console.WriteLine($"Query filters set up correctly. Result count for both queries: {allPosts.Count}.");
            }
            else
            {
                Console.WriteLine("Unexpected discrepancy due to query filters and required navigations interaction.");
                Console.WriteLine($"All posts count: {allPosts.Count}.");
                Console.WriteLine($"All posts with blogs included count: {allPostsWithBlogsIncluded.Count}.");
            }
        }
    }

    public class QueryFiltersAndRequiredNavigationsContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=Querying.QueryFilters.BloggingRequired;Trusted_Connection=True;ConnectRetryCount=0");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var setup = "OptionalNav";
            if (setup == "Faulty")
            {
                // Incorrect setup - Required navigation used to reference entity that has query filter defined,
                // but no query filter for the entity on the other side of the navigation.
                #region IncorrectFilter
                modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired();
                modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
                #endregion
            }
            else if (setup == "OptionalNav")
            {
                // The relationship is marked as optional so dependent can exist even if principal is filtered out.
                #region OptionalNavigation
                modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired(false);
                modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
                #endregion
            }
            else if (setup == "NavigationInFilter")
            {
                #region NavigationInFilter
                modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog);
                modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Posts.Count > 0);
                modelBuilder.Entity<Post>().HasQueryFilter(p => p.Title.Contains("fish"));
                #endregion
            }
            else
            {
                // The relationship is still required but there is a matching filter configured on dependent side too,
                // which matches principal side. So if principal is filtered out, the dependent would also be.
                #region MatchingFilters
                modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired();
                modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
                modelBuilder.Entity<Post>().HasQueryFilter(p => p.Blog.Url.Contains("fish"));
                #endregion
            }
        }
    }

    public class Blog
    {
    #pragma warning disable IDE0051, CS0169 // Remove unused private members
        private string _tenantId;
    #pragma warning restore IDE0051, CS0169 // Remove unused private members

        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }

        public Blog Blog { get; set; }
    }
}
