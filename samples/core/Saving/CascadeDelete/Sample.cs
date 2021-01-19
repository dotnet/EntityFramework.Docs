using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.CascadeDelete
{
    public class Sample
    {
        public static void Run()
        {
            DeleteBehaviorSample(DeleteBehavior.Cascade, true);
            DeleteBehaviorSample(DeleteBehavior.ClientSetNull, true);
            DeleteBehaviorSample(DeleteBehavior.SetNull, true);
            DeleteBehaviorSample(DeleteBehavior.Restrict, true);

            DeleteBehaviorSample(DeleteBehavior.Cascade, false);
            DeleteBehaviorSample(DeleteBehavior.ClientSetNull, false);
            DeleteBehaviorSample(DeleteBehavior.SetNull, false);
            DeleteBehaviorSample(DeleteBehavior.Restrict, false);

            DeleteOrphansSample(DeleteBehavior.Cascade, true);
            DeleteOrphansSample(DeleteBehavior.ClientSetNull, true);
            DeleteOrphansSample(DeleteBehavior.SetNull, true);
            DeleteOrphansSample(DeleteBehavior.Restrict, true);

            DeleteOrphansSample(DeleteBehavior.Cascade, false);
            DeleteOrphansSample(DeleteBehavior.ClientSetNull, false);
            DeleteOrphansSample(DeleteBehavior.SetNull, false);
            DeleteOrphansSample(DeleteBehavior.Restrict, false);
        }

        private static void DeleteBehaviorSample(DeleteBehavior deleteBehavior, bool requiredRelationship)
        {
            Console.WriteLine(
                $"Test using DeleteBehavior.{deleteBehavior} with {(requiredRelationship ? "required" : "optional")} relationship:");

            InitializeDatabase(requiredRelationship);

            using var context = new BloggingContext(deleteBehavior, requiredRelationship);

            #region DeleteBehaviorVariations
            var blog = context.Blogs.Include(b => b.Posts).First();
            var posts = blog.Posts.AsQueryable().ToList();

            DumpEntities("  After loading entities:", context, blog, posts);

            context.Remove(blog);

            DumpEntities($"  After deleting blog '{blog.BlogId}':", context, blog, posts);

            try
            {
                Console.WriteLine();
                Console.WriteLine("  Saving changes:");

                context.SaveChanges();

                DumpSql();

                DumpEntities("  After SaveChanges:", context, blog, posts);
            }
            catch (Exception e)
            {
                DumpSql();

                Console.WriteLine();
                Console.WriteLine(
                    $"  SaveChanges threw {e.GetType().Name}: {(e is DbUpdateException ? e.InnerException.Message : e.Message)}");
            }
            #endregion

            Console.WriteLine();
        }

        private static void DeleteOrphansSample(DeleteBehavior deleteBehavior, bool requiredRelationship)
        {
            Console.WriteLine(
                $"Test deleting orphans with DeleteBehavior.{deleteBehavior} and {(requiredRelationship ? "a required" : "an optional")} relationship:");

            InitializeDatabase(requiredRelationship);

            using var context = new BloggingContext(deleteBehavior, requiredRelationship);

            #region DeleteOrphansVariations
            var blog = context.Blogs.Include(b => b.Posts).First();
            var posts = blog.Posts.AsQueryable().ToList();

            DumpEntities("  After loading entities:", context, blog, posts);

            blog.Posts.Clear();

            DumpEntities("  After making posts orphans:", context, blog, posts);

            try
            {
                Console.WriteLine();
                Console.WriteLine("  Saving changes:");

                context.SaveChanges();

                DumpSql();

                DumpEntities("  After SaveChanges:", context, blog, posts);
            }
            catch (Exception e)
            {
                DumpSql();

                Console.WriteLine();
                Console.WriteLine(
                    $"  SaveChanges threw {e.GetType().Name}: {(e is DbUpdateException ? e.InnerException.Message : e.Message)}");
            }
            #endregion

            Console.WriteLine();
        }

        private static void InitializeDatabase(bool requiredRelationship)
        {
            using var context = new BloggingContext(DeleteBehavior.ClientSetNull, requiredRelationship);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Blogs.Add(
                new Blog
                {
                    Url = "http://sample.com",
                    Posts = new List<Post> { new Post { Title = "Saving Data with EF" }, new Post { Title = "Cascade Delete with EF" } }
                });

            context.SaveChanges();
        }

        private static void DumpEntities(string message, BloggingContext context, Blog blog, IList<Post> posts)
        {
            Console.WriteLine();
            Console.WriteLine(message);

            var blogEntry = context.Entry(blog);

            Console.WriteLine($"    Blog '{blog.BlogId}' is in state {blogEntry.State} with {posts.Count} posts referenced.");

            foreach (var post in posts)
            {
                var postEntry = context.Entry(post);

                Console.WriteLine(
                    $"      Post '{post.PostId}' is in state {postEntry.State} " +
                    $"with FK '{post.BlogId?.ToString() ?? "null"}' and {(post.Blog == null ? "no reference to a blog." : $"reference to blog '{post.BlogId}'.")}");
            }
        }

        private static void DumpSql()
        {
            foreach (var logMessage in BloggingContext.LogMessages)
            {
                Console.WriteLine("    " + logMessage);
            }
        }
    }
}
