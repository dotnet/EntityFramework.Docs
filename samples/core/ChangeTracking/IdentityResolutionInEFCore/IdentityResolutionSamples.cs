using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Updates
{
    public static class IdentityResolutionSamples
    {
        public static void Identity_Resolution_in_EF_Core_1()
        {
            Console.WriteLine($">>>> Sample: {nameof(Identity_Resolution_in_EF_Core_1)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            #region Identity_Resolution_in_EF_Core_1
            using var context = new BlogsContext();

            var blogA = context.Blogs.Single(e => e.Id == 1);
            var blogB = new Blog { Id = 1, Name = ".NET Blog (All new!)" };

            try
            {
                context.Update(blogB); // This will throw
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
            #endregion

            Console.WriteLine();
        }

        public static void Updating_an_entity_1()
        {
            Console.WriteLine($">>>> Sample: {nameof(Updating_an_entity_1)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            UpdateFromHttpPost1(
                new Blog { Id = 1, Name = ".NET Blog (All new!)", Summary = "Posts about .NET" });

            Console.WriteLine();
        }

        #region Updating_an_entity_1
        public static void UpdateFromHttpPost1(Blog blog)
        {
            using var context = new BlogsContext();

            context.Update(blog);

            context.SaveChanges();
        }
        #endregion

        public static void Updating_an_entity_2()
        {
            Console.WriteLine($">>>> Sample: {nameof(Updating_an_entity_2)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            UpdateFromHttpPost2(
                new Blog { Id = 1, Name = ".NET Blog (All new!)", Summary = "Posts about .NET" });

            Console.WriteLine();
        }

        #region Updating_an_entity_2
        public static void UpdateFromHttpPost2(Blog blog)
        {
            using var context = new BlogsContext();

            var trackedBlog = context.Blogs.Find(blog.Id);

            trackedBlog.Name = blog.Name;
            trackedBlog.Summary = blog.Summary;

            context.SaveChanges();
        }
        #endregion

        public static void Updating_an_entity_3()
        {
            Console.WriteLine($">>>> Sample: {nameof(Updating_an_entity_3)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            UpdateFromHttpPost3(
                new Blog { Id = 1, Name = ".NET Blog (All new!)", Summary = "Posts about .NET" });

            Console.WriteLine();
        }

        #region Updating_an_entity_3
        public static void UpdateFromHttpPost3(Blog blog)
        {
            using var context = new BlogsContext();

            var trackedBlog = context.Blogs.Find(blog.Id);

            context.Entry(trackedBlog).CurrentValues.SetValues(blog);

            context.SaveChanges();
        }
        #endregion

        public static void Updating_an_entity_4()
        {
            Console.WriteLine($">>>> Sample: {nameof(Updating_an_entity_4)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            UpdateFromHttpPost4(
                new BlogDto { Id = 1, Name = ".NET Blog (All new!)", Summary = "Posts about .NET" });

            Console.WriteLine();
        }

        #region Updating_an_entity_4
        public static void UpdateFromHttpPost4(BlogDto dto)
        {
            using var context = new BlogsContext();

            var trackedBlog = context.Blogs.Find(dto.Id);

            context.Entry(trackedBlog).CurrentValues.SetValues(dto);

            context.SaveChanges();
        }
        #endregion

        public static void Updating_an_entity_5()
        {
            Console.WriteLine($">>>> Sample: {nameof(Updating_an_entity_5)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            UpdateFromHttpPost5(
                new Dictionary<string, object> { ["Id"] = 1, ["Name"] = ".NET Blog (All new!)", ["Summary"] = "Posts about .NET" });

            Console.WriteLine();
        }

        #region Updating_an_entity_5
        public static void UpdateFromHttpPost5(Dictionary<string, object> propertyValues)
        {
            using var context = new BlogsContext();

            var trackedBlog = context.Blogs.Find(propertyValues["Id"]);

            context.Entry(trackedBlog).CurrentValues.SetValues(propertyValues);

            context.SaveChanges();
        }
        #endregion

        public static void Updating_an_entity_6()
        {
            Console.WriteLine($">>>> Sample: {nameof(Updating_an_entity_6)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            UpdateFromHttpPost6(
                new Blog { Id = 1, Name = ".NET Blog (All new!)", Summary = "Posts about .NET" },
                new Dictionary<string, object> { ["Id"] = 1, ["Name"] = ".NET Blog", ["Summary"] = "Posts about .NET" });

            Console.WriteLine();
        }

        #region Updating_an_entity_6
        public static void UpdateFromHttpPost6(Blog blog, Dictionary<string, object> originalValues)
        {
            using var context = new BlogsContext();

            context.Attach(blog);
            context.Entry(blog).OriginalValues.SetValues(originalValues);

            context.SaveChanges();
        }
        #endregion

        public static void Failing_to_set_key_values_1()
        {
            Console.WriteLine($">>>> Sample: {nameof(Failing_to_set_key_values_1)}");
            Console.WriteLine();

            #region Failing_to_set_key_values_1
            using var context = new BlogsContext();

            context.Add(new Pet { Name = "Smokey" });

            try
            {
                context.Add(new Pet { Name = "Clippy" }); // This will throw
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
            #endregion
        }
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new BlogsContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void PopulateDatabase()
        {
            using var context = new BlogsContext(quiet: true);

            context.Add(
                new Blog
                {
                    Name = ".NET Blog",
                    Posts =
                    {
                        new Post
                        {
                            Title = "Announcing the Release of EF Core 5.0",
                            Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                        },
                        new Post
                        {
                            Title = "Announcing F# 5",
                            Content = "F# 5 is the latest version of F#, the functional programming language..."
                        },
                    }
                });

            context.SaveChanges();
        }
    }

    #region Pet
    public class Pet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
    #endregion

    public class Customer
    {
        #region OrdersCollection
        public ICollection<Order> Orders { get; set; }
            = new HashSet<Order>(ReferenceEqualityComparer.Instance);
        #endregion
    }

    public class Order
    {
    }

    public class BlogDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }

        public IList<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int? BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class BlogsContext : DbContext
    {
        private readonly bool _quiet;

        public BlogsContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Pet> Pets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlite("DataSource=test.db");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
