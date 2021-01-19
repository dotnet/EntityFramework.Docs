using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Required
{
    public static class RequiredDependentsSamples
    {
        public static void Required_relationship_with_dependents_children_loaded()
        {
            Console.WriteLine("#### Required relationship with dependents/children loaded");
            Console.WriteLine();

            var deleteResults = Helpers.GatherData(c => c.Remove(c.Blogs.Include(e => e.Posts).Single()));
            var severResults = Helpers.GatherData(c => c.Blogs.Include(e => e.Posts).Single().Posts.Clear());

            Console.WriteLine(
                $"| `{"DeleteBehavior".PadRight(16)} | {"On deleting principal/parent".PadRight(40)} | On severing from principal/parent");
            Console.WriteLine("|:------------------|------------------------------------------|----------------------------------------");
            foreach (var deleteBehavior in DeleteBehaviors)
            {
                Console.WriteLine(
                    $"| `{(deleteBehavior + "`").PadRight(16)} | {deleteResults[deleteBehavior].PadRight(40)} | {severResults[deleteBehavior]}");
            }

            Console.WriteLine();
        }

        public static void Required_relationship_with_dependents_children_not_loaded()
        {
            Console.WriteLine("#### Required relationship with dependents/children not loaded");
            Console.WriteLine();

            var deleteResults = Helpers.GatherData(c => c.Remove(c.Blogs.Single()));

            Console.WriteLine(
                $"| `{"DeleteBehavior".PadRight(16)} | {"On deleting principal/parent".PadRight(40)} | On severing from principal/parent");
            Console.WriteLine("|:------------------|------------------------------------------|----------------------------------------");
            foreach (var deleteBehavior in DeleteBehaviors)
            {
                Console.WriteLine($"| `{(deleteBehavior + "`").PadRight(16)} | {deleteResults[deleteBehavior].PadRight(40)} | N/A");
            }

            Console.WriteLine();
        }

        public static DeleteBehavior[] DeleteBehaviors { get; }
            =
            {
                DeleteBehavior.Cascade, DeleteBehavior.Restrict, DeleteBehavior.NoAction, DeleteBehavior.SetNull,
                DeleteBehavior.ClientSetNull, DeleteBehavior.ClientCascade, DeleteBehavior.ClientNoAction
            };

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

            public int BlogId { get; set; }
            public Blog Blog { get; set; }
        }
        #endregion

        public static class Helpers
        {
            public static void RecreateCleanDatabase(RequiredBlogsContext context)
            {
                using (context)
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
            }

            public static void PopulateDatabase(RequiredBlogsContext context)
            {
                using (context)
                {
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

            public static Dictionary<DeleteBehavior, string> GatherData(Action<RequiredBlogsContext> action)
            {
                var results = new Dictionary<DeleteBehavior, string>();

                foreach (var deleteBehavior in DeleteBehaviors)
                {
                    try
                    {
                        RecreateCleanDatabase(new RequiredBlogsContext(deleteBehavior));
                        PopulateDatabase(new RequiredBlogsContext(deleteBehavior));
                    }
                    catch (Exception e)
                    {
                        results[deleteBehavior] = $"`{e.GetType().Name}`";
                        continue;
                    }

                    try
                    {
                        using var context = new RequiredBlogsContext(deleteBehavior);

                        action(context);

                        context.ChangeTracker.DetectChanges();

                        var deletingPosts = context.ChangeTracker.Entries<Post>().Any(e => e.State == EntityState.Deleted);
                        var settingFksToNull = context.ChangeTracker.Entries<Post>().Any(e => e.State == EntityState.Modified);

                        context.SaveChanges();

                        var deletedPosts = !context.Posts.AsNoTracking().Any();

                        results[deleteBehavior] =
                            deletingPosts
                                ? "Dependents deleted by EF Core"
                                : deletedPosts
                                    ? "Dependents deleted by database"
                                    : settingFksToNull
                                        ? "Dependent FKs set to null by EF Core"
                                        : "Dependent FKs set to null by database";
                    }
                    catch (Exception e)
                    {
                        results[deleteBehavior] = $"`{e.GetType().Name}`";
                    }
                }

                return results;
            }
        }

        public class RequiredBlogsContext : DbContext
        {
            private readonly DeleteBehavior _deleteBehavior;
            private readonly bool _quiet;

            public RequiredBlogsContext(DeleteBehavior deleteBehavior, bool quiet = true)
            {
                _deleteBehavior = deleteBehavior;
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
                    .OnDelete(_deleteBehavior);
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder
                    .EnableServiceProviderCaching(false)
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
}
