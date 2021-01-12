using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IntroOptional
{
    public static class IntroOptionalSamples
    {
        public static void Deleting_principal_parent_1b()
        {
            Console.WriteLine($">>>> Sample: {nameof(Deleting_principal_parent_1b)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            #region Deleting_principal_parent_1b
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            context.Remove(blog);

            context.SaveChanges();
            #endregion

            Console.WriteLine();
        }

        public static void Severing_a_relationship_1b()
        {
            Console.WriteLine($">>>> Sample: {nameof(Severing_a_relationship_1b)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            #region Severing_a_relationship_1b
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            foreach (var post in blog.Posts)
            {
                post.Blog = null;
            }

            context.SaveChanges();
            #endregion

            Console.WriteLine();
        }

        public static void Severing_a_relationship_2b()
        {
            Console.WriteLine($">>>> Sample: {nameof(Severing_a_relationship_2b)}");
            Console.WriteLine();

            Helpers.RecreateCleanDatabase();
            Helpers.PopulateDatabase();

            #region Severing_a_relationship_2b
            using var context = new BlogsContext();

            var blog = context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).First();

            blog.Posts.Clear();

            context.SaveChanges();
            #endregion

            Console.WriteLine();
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

    #region Model
    public class Blog
    {
        public int Id { get; set; }

        public string Name { get; set; }

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
    #endregion

    public class BlogsContext : DbContext
    {
        private readonly bool _quiet;

        public BlogsContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Blog>()
                .HasMany(e => e.Posts)
                .WithOne(e => e.Blog)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Scratch;ConnectRetryCount=0");
            //.UseSqlite("DataSource=test.db");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
