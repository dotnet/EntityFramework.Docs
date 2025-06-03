using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.CascadeDelete;

public class Sample
{
    public static async Task Run()
    {
        await DeleteBehaviorSample(DeleteBehavior.Cascade, true);
        await DeleteBehaviorSample(DeleteBehavior.ClientSetNull, true);
        await DeleteBehaviorSample(DeleteBehavior.SetNull, true);
        await DeleteBehaviorSample(DeleteBehavior.Restrict, true);

        await DeleteBehaviorSample(DeleteBehavior.Cascade, false);
        await DeleteBehaviorSample(DeleteBehavior.ClientSetNull, false);
        await DeleteBehaviorSample(DeleteBehavior.SetNull, false);
        await DeleteBehaviorSample(DeleteBehavior.Restrict, false);

        await DeleteOrphansSample(DeleteBehavior.Cascade, true);
        await DeleteOrphansSample(DeleteBehavior.ClientSetNull, true);
        await DeleteOrphansSample(DeleteBehavior.SetNull, true);
        await DeleteOrphansSample(DeleteBehavior.Restrict, true);

        await DeleteOrphansSample(DeleteBehavior.Cascade, false);
        await DeleteOrphansSample(DeleteBehavior.ClientSetNull, false);
        await DeleteOrphansSample(DeleteBehavior.SetNull, false);
        await DeleteOrphansSample(DeleteBehavior.Restrict, false);
    }

    private static async Task DeleteBehaviorSample(DeleteBehavior deleteBehavior, bool requiredRelationship)
    {
        Console.WriteLine(
            $"Test using DeleteBehavior.{deleteBehavior} with {(requiredRelationship ? "required" : "optional")} relationship:");

        await InitializeDatabase(requiredRelationship);

        using var context = new BloggingContext(deleteBehavior, requiredRelationship);

        #region DeleteBehaviorVariations
        var blog = await context.Blogs.Include(b => b.Posts).FirstAsync();
        var posts = blog.Posts.ToList();

        DumpEntities("  After loading entities:", context, blog, posts);

        context.Remove(blog);

        DumpEntities($"  After deleting blog '{blog.BlogId}':", context, blog, posts);

        try
        {
            Console.WriteLine();
            Console.WriteLine("  Saving changes:");

            await context.SaveChangesAsync();

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

    private static async Task DeleteOrphansSample(DeleteBehavior deleteBehavior, bool requiredRelationship)
    {
        Console.WriteLine(
            $"Test deleting orphans with DeleteBehavior.{deleteBehavior} and {(requiredRelationship ? "a required" : "an optional")} relationship:");

        await InitializeDatabase(requiredRelationship);

        using var context = new BloggingContext(deleteBehavior, requiredRelationship);

        #region DeleteOrphansVariations
        var blog = await context.Blogs.Include(b => b.Posts).FirstAsync();
        var posts = blog.Posts.ToList();

        DumpEntities("  After loading entities:", context, blog, posts);

        blog.Posts.Clear();

        DumpEntities("  After making posts orphans:", context, blog, posts);

        try
        {
            Console.WriteLine();
            Console.WriteLine("  Saving changes:");

            await context.SaveChangesAsync();

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

    private static async Task InitializeDatabase(bool requiredRelationship)
    {
        using var context = new BloggingContext(DeleteBehavior.ClientSetNull, requiredRelationship);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.Blogs.Add(
            new Blog
            {
                Url = "http://sample.com",
                Posts = new List<Post> { new Post { Title = "Saving Data with EF" }, new Post { Title = "Cascade Delete with EF" } }
            });

        await context.SaveChangesAsync();
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
