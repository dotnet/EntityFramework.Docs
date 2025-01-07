using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Disconnected;

public class Sample
{
    public static async Task Run()
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        await IsItNew();
        await InsertAndUpdateSingleEntity();
        await InsertOrUpdateSingleEntityStoreGenerated();
        await InsertOrUpdateSingleEntityFind();
        await InsertAndUpdateGraph();
        await InsertOrUpdateGraphStoreGenerated();
        await InsertOrUpdateGraphFind();
        await InsertUpdateOrDeleteGraphFind();
        await InsertUpdateOrDeleteTrackGraph();
    }

    private static async Task IsItNew()
    {
        Console.WriteLine();
        Console.WriteLine("Show entity-specific check for key set:");
        using (var context = new BloggingContext())
        {
            var blog = new Blog { Url = "http://sample.com" };

            // Key is not set for a new entity
            Console.WriteLine($"  Blog entity is {(IsItNew(blog) ? "new" : "existing")}.");

            context.Add(blog);
            await context.SaveChangesAsync();

            // Key is now set
            Console.WriteLine($"  Blog entity is {(IsItNew(blog) ? "new" : "existing")}.");
        }

        Console.WriteLine();
        Console.WriteLine("Show general IsKeySet:");
        using (var context = new BloggingContext())
        {
            var blog = new Blog { Url = "http://sample.com" };

            // Key is not set for a new entity
            Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");

            context.Add(blog);
            await context.SaveChangesAsync();

            // Key is now set
            Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");
        }

        Console.WriteLine();
        Console.WriteLine("Show key set on Add:");
        using (var context = new BloggingContext())
        {
            var blog = new Blog { Url = "http://sample.com" };

            // Key is not set for a new entity
            Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");

            context.Add(blog);

            // Key is set as soon as Add assigns a key, even if it is temporary
            Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");
        }

        Console.WriteLine();
        Console.WriteLine("Show using query to check for new entity:");
        using (var context = new BloggingContext())
        {
            var blog = new Blog { Url = "http://sample.com" };

            Console.WriteLine($"  Blog entity is {(await IsItNew(context, blog) ? "new" : "existing")}.");

            context.Add(blog);
            await context.SaveChangesAsync();

            Console.WriteLine($"  Blog entity is {(await IsItNew(context, blog) ? "new" : "existing")}.");
        }
    }

    private static async Task InsertAndUpdateSingleEntity()
    {
        Console.WriteLine();
        Console.WriteLine("Save single entity with explicit insert or update:");
        var blog = new Blog { Url = "http://sample.com" };

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Inserting with URL {blog.Url}");
            await Insert(context, blog);
        }

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Found with URL {(await context.Blogs.SingleAsync(b => b.BlogId == blog.BlogId)).Url}");
        }

        using (var context = new BloggingContext())
        {
            blog.Url = "https://sample.com";
            Console.WriteLine($"  Updating with URL {blog.Url}");
            await Update(context, blog);
        }

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Found with URL {((await context.Blogs.SingleAsync(b => b.BlogId == blog.BlogId))).Url}");
        }
    }

    private static async Task InsertOrUpdateSingleEntityStoreGenerated()
    {
        Console.WriteLine();
        Console.WriteLine("Save single entity with auto-generated key:");
        var blog = new Blog { Url = "http://sample.com" };

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Inserting with URL {blog.Url}");
            await InsertOrUpdate(context, (object)blog);
        }

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Found with URL {(await context.Blogs.SingleAsync(b => b.BlogId == blog.BlogId)).Url}");
        }

        using (var context = new BloggingContext())
        {
            blog.Url = "https://sample.com";
            Console.WriteLine($"  Updating with URL {blog.Url}");
            await InsertOrUpdate(context, (object)blog);
        }

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Found with URL {((await context.Blogs.SingleAsync(b => b.BlogId == blog.BlogId))).Url}");
        }
    }

    private static async Task InsertOrUpdateSingleEntityFind()
    {
        Console.WriteLine();
        Console.WriteLine("Save single entity with any kind of key:");
        var blog = new Blog { Url = "http://sample.com" };

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Inserting with URL {blog.Url}");
            await InsertOrUpdate(context, blog);
        }

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Found with URL {(await context.Blogs.SingleAsync(b => b.BlogId == blog.BlogId)).Url}");
        }

        using (var context = new BloggingContext())
        {
            blog.Url = "https://sample.com";
            Console.WriteLine($"  Updating with URL {blog.Url}");
            await InsertOrUpdate(context, blog);
        }

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Found with URL {(await context.Blogs.SingleAsync(b => b.BlogId == blog.BlogId)).Url}");
        }
    }

    private static async Task InsertAndUpdateGraph()
    {
        Console.WriteLine();
        Console.WriteLine("Save graph with explicit insert or update:");
        var blog = CreateBlogAndPosts();

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
            await InsertGraph(context, blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
        }

        using (var context = new BloggingContext())
        {
            blog.Url = "https://sample.com";
            blog.Posts[0].Title = "Post A";
            blog.Posts[1].Title = "Post B";

            Console.WriteLine($"  Updating with URL {blog.Url}");
            await UpdateGraph(context, blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
        }
    }

    private static async Task InsertOrUpdateGraphStoreGenerated()
    {
        Console.WriteLine();
        Console.WriteLine("Save graph with auto-generated key:");
        var blog = CreateBlogAndPosts();

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
            await InsertOrUpdateGraph(context, (object)blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
        }

        using (var context = new BloggingContext())
        {
            blog.Url = "https://sample.com";
            blog.Posts[0].Title = "Post A";
            blog.Posts[1].Title = "Post B";
            blog.Posts.Add(new Post { Title = "New Post" });

            Console.WriteLine($"  Updating with URL {blog.Url}");
            await InsertOrUpdateGraph(context, (object)blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}, {read.Posts[2].Title}");
        }
    }

    private static async Task InsertOrUpdateGraphFind()
    {
        Console.WriteLine();
        Console.WriteLine("Save graph with any kind of key:");
        var blog = CreateBlogAndPosts();

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
            await InsertOrUpdateGraph(context, blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
        }

        using (var context = new BloggingContext())
        {
            blog.Url = "https://sample.com";
            blog.Posts[0].Title = "Post A";
            blog.Posts[1].Title = "Post B";
            blog.Posts.Add(new Post { Title = "New Post" });

            Console.WriteLine($"  Updating with URL {blog.Url}");
            await InsertOrUpdateGraph(context, blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}, {read.Posts[2].Title}");
        }
    }

    private static async Task InsertUpdateOrDeleteGraphFind()
    {
        Console.WriteLine();
        Console.WriteLine("Save graph with deletes and any kind of key:");
        var blog = CreateBlogAndPosts();

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
            await InsertUpdateOrDeleteGraph(context, blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
        }

        using (var context = new BloggingContext())
        {
            blog.Url = "https://sample.com";
            blog.Posts[0].Title = "Post A";
            blog.Posts.Remove(blog.Posts[1]);
            blog.Posts.Add(new Post { Title = "New Post" });

            Console.WriteLine($"  Updating with URL {blog.Url}");
            await InsertUpdateOrDeleteGraph(context, blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
        }
    }

    private static async Task InsertUpdateOrDeleteTrackGraph()
    {
        Console.WriteLine();
        Console.WriteLine("Save graph using TrackGraph:");
        var blog = CreateBlogAndPosts();
        blog.IsNew = true;
        blog.Posts[0].IsNew = true;
        blog.Posts[1].IsNew = true;

        using (var context = new BloggingContext())
        {
            Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
            await SaveAnnotatedGraph(context, blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
        }

        blog.IsNew = false;
        blog.Posts[0].IsNew = false;
        blog.Posts[1].IsNew = false;

        using (var context = new BloggingContext())
        {
            blog.Url = "https://sample.com";
            blog.IsChanged = true;
            blog.Posts[0].Title = "Post A";
            blog.Posts[0].IsDeleted = true;
            blog.Posts[1].Title = "Post B";
            blog.Posts.Add(new Post { Title = "New Post", IsNew = true });

            Console.WriteLine($"  Updating with URL {blog.Url}");
            await SaveAnnotatedGraph(context, blog);
        }

        using (var context = new BloggingContext())
        {
            var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
            Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
        }
    }

    #region IsItNewSimple
    public static bool IsItNew(Blog blog)
        => blog.BlogId == 0;
    #endregion

    #region IsItNewGeneral
    public static bool IsItNew(DbContext context, object entity)
        => !context.Entry(entity).IsKeySet;
    #endregion

    #region IsItNewQuery
    public static async Task<bool> IsItNew(BloggingContext context, Blog blog)
        => (await context.Blogs.FindAsync(blog.BlogId)) == null;
    #endregion

    #region InsertAndUpdateSingleEntity
    public static async Task Insert(DbContext context, object entity)
    {
        context.Add(entity);
        await context.SaveChangesAsync();
    }

    public static async Task Update(DbContext context, object entity)
    {
        context.Update(entity);
        await context.SaveChangesAsync();
    }
    #endregion

    #region InsertOrUpdateSingleEntity
    public static async Task InsertOrUpdate(DbContext context, object entity)
    {
        context.Update(entity);
        await context.SaveChangesAsync();
    }
    #endregion

    #region InsertOrUpdateSingleEntityWithFind
    public static async Task InsertOrUpdate(BloggingContext context, Blog blog)
    {
        var existingBlog = await context.Blogs.FindAsync(blog.BlogId);
        if (existingBlog == null)
        {
            context.Add(blog);
        }
        else
        {
            context.Entry(existingBlog).CurrentValues.SetValues(blog);
        }

        await context.SaveChangesAsync();
    }
    #endregion

    private static Blog CreateBlogAndPosts()
    {
        #region CreateBlogAndPosts
        var blog = new Blog
        {
            Url = "http://sample.com", Posts = new List<Post> { new Post { Title = "Post 1" }, new Post { Title = "Post 2" }, }
        };
        #endregion

        return blog;
    }

    #region InsertGraph
    public static async Task InsertGraph(DbContext context, object rootEntity)
    {
        context.Add(rootEntity);
        await context.SaveChangesAsync();
    }
    #endregion

    #region UpdateGraph
    public static async Task UpdateGraph(DbContext context, object rootEntity)
    {
        context.Update(rootEntity);
        await context.SaveChangesAsync();
    }
    #endregion

    #region InsertOrUpdateGraph
    public static async Task InsertOrUpdateGraph(DbContext context, object rootEntity)
    {
        context.Update(rootEntity);
        await context.SaveChangesAsync();
    }
    #endregion

    #region InsertOrUpdateGraphWithFind
    public static async Task InsertOrUpdateGraph(BloggingContext context, Blog blog)
    {
        var existingBlog = await context.Blogs
            .Include(b => b.Posts)
            .FirstOrDefaultAsync(b => b.BlogId == blog.BlogId);

        if (existingBlog == null)
        {
            context.Add(blog);
        }
        else
        {
            context.Entry(existingBlog).CurrentValues.SetValues(blog);
            foreach (var post in blog.Posts)
            {
                var existingPost = existingBlog.Posts
                    .FirstOrDefault(p => p.PostId == post.PostId);

                if (existingPost == null)
                {
                    existingBlog.Posts.Add(post);
                }
                else
                {
                    context.Entry(existingPost).CurrentValues.SetValues(post);
                }
            }
        }

        await context.SaveChangesAsync();
    }
    #endregion

    #region InsertUpdateOrDeleteGraphWithFind
    public static async Task InsertUpdateOrDeleteGraph(BloggingContext context, Blog blog)
    {
        var existingBlog = await context.Blogs
            .Include(b => b.Posts)
            .FirstOrDefaultAsync(b => b.BlogId == blog.BlogId);

        if (existingBlog == null)
        {
            context.Add(blog);
        }
        else
        {
            context.Entry(existingBlog).CurrentValues.SetValues(blog);
            foreach (var post in blog.Posts)
            {
                var existingPost = existingBlog.Posts
                    .FirstOrDefault(p => p.PostId == post.PostId);

                if (existingPost == null)
                {
                    existingBlog.Posts.Add(post);
                }
                else
                {
                    context.Entry(existingPost).CurrentValues.SetValues(post);
                }
            }

            foreach (var post in existingBlog.Posts)
            {
                if (!blog.Posts.Any(p => p.PostId == post.PostId))
                {
                    context.Remove(post);
                }
            }
        }

        await context.SaveChangesAsync();
    }
    #endregion

    #region TrackGraph
    public static async Task SaveAnnotatedGraph(DbContext context, object rootEntity)
    {
        context.ChangeTracker.TrackGraph(
            rootEntity,
            n =>
            {
                var entity = (EntityBase)n.Entry.Entity;
                n.Entry.State = entity.IsNew
                    ? EntityState.Added
                    : entity.IsChanged
                        ? EntityState.Modified
                        : entity.IsDeleted
                            ? EntityState.Deleted
                            : EntityState.Unchanged;
            });

        await context.SaveChangesAsync();
    }
    #endregion
}
