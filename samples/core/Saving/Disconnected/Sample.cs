using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Disconnected
{
    public class Sample
    {
        public static async Task RunAsync()
        {
            await using (var context = new BloggingContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            await IsItNewAsync();
            await InsertAndUpdateSingleEntityAsync();
            await InsertOrUpdateSingleEntityStoreGeneratedAsync();
            await InsertOrUpdateSingleEntityFindAsync();
            await InsertAndUpdateGraphAsync();
            await InsertOrUpdateGraphStoreGeneratedAsync();
            await InsertOrUpdateGraphFindAsync();
            await InsertUpdateOrDeleteGraphFindAsync();
            await InsertUpdateOrDeleteTrackGraphAsync();
        }

        private static async Task IsItNewAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Show entity-specific check for key set:");
            await using (var context = new BloggingContext())
            {
                var blog = new Blog {Url = "http://sample.com"};

                // Key is not set for a new entity
                Console.WriteLine($"  Blog entity is {(IsItNew(blog) ? "new" : "existing")}.");

                context.Add(blog);
                await context.SaveChangesAsync();

                // Key is now set
                Console.WriteLine($"  Blog entity is {(IsItNew(blog) ? "new" : "existing")}.");
            }

            Console.WriteLine();
            Console.WriteLine("Show general IsKeySet:");
            await using (var context = new BloggingContext())
            {
                var blog = new Blog {Url = "http://sample.com"};

                // Key is not set for a new entity
                Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");

                context.Add(blog);
                await context.SaveChangesAsync();

                // Key is now set
                Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");
            }

            Console.WriteLine();
            Console.WriteLine("Show key set on Add:");
            await using (var context = new BloggingContext())
            {
                var blog = new Blog {Url = "http://sample.com"};

                // Key is not set for a new entity
                Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");

                context.Add(blog);

                // Key is set as soon as Add assigns a key, even if it is temporary
                Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");
            }

            Console.WriteLine();
            Console.WriteLine("Show using query to check for new entity:");
            await using (var context = new BloggingContext())
            {
                var blog = new Blog {Url = "http://sample.com"};

                Console.WriteLine($"  Blog entity is {(IsItNewAsync(context, blog) ? "new" : "existing")}.");

                context.Add(blog);
                await context.SaveChangesAsync();

                Console.WriteLine($"  Blog entity is {(IsItNewAsync(context, blog) ? "new" : "existing")}.");
            }
        }

        private static async Task InsertAndUpdateSingleEntityAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Save single entity with explicit insert or update:");
            var blog = new Blog { Url = "http://sample.com" };

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url}");
                await InsertAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }

            await using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                Console.WriteLine($"  Updating with URL {blog.Url}");
                await UpdateAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {(await context.Blogs.SingleAsync(b => b.BlogId == blog.BlogId)).Url}");
            }
        }

        private static async Task InsertOrUpdateSingleEntityStoreGeneratedAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Save single entity with auto-generated key:");
            var blog = new Blog { Url = "http://sample.com" };

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url}");
                await InsertOrUpdateAsync(context, (object)blog);
            }

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }

            await using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                Console.WriteLine($"  Updating with URL {blog.Url}");
                await InsertOrUpdateAsync(context, (object)blog);
            }

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {(await context.Blogs.SingleAsync(b => b.BlogId == blog.BlogId)).Url}");
            }
        }

        private static async Task InsertOrUpdateSingleEntityFindAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Save single entity with any kind of key:");
            var blog = new Blog { Url = "http://sample.com" };

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url}");
                await InsertOrUpdateAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }

            await using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                Console.WriteLine($"  Updating with URL {blog.Url}");
                await InsertOrUpdateAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }
        }

        private static async Task InsertAndUpdateGraphAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph with explicit insert or update:");
            var blog = CreateBlogAndPosts();

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                await InsertGraphAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            await using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.Posts[0].Title = "Post A";
                blog.Posts[1].Title = "Post B";

                Console.WriteLine($"  Updating with URL {blog.Url}");
                await UpdateGraphAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }
        }

        private static async Task InsertOrUpdateGraphStoreGeneratedAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph with auto-generated key:");
            var blog = CreateBlogAndPosts();

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                await InsertOrUpdateGraphAsync(context, (object)blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            await using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.Posts[0].Title = "Post A";
                blog.Posts[1].Title = "Post B";
                blog.Posts.Add(new Post { Title = "New Post" });

                Console.WriteLine($"  Updating with URL {blog.Url}");
                await InsertOrUpdateGraphAsync(context, (object)blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}, {read.Posts[2].Title}");
            }
        }

        private static async Task InsertOrUpdateGraphFindAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph with any kind of key:");
            var blog = CreateBlogAndPosts();

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                await InsertOrUpdateGraphAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            await using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.Posts[0].Title = "Post A";
                blog.Posts[1].Title = "Post B";
                blog.Posts.Add(new Post { Title = "New Post" });

                Console.WriteLine($"  Updating with URL {blog.Url}");
                await InsertOrUpdateGraphAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}, {read.Posts[2].Title}");
            }
        }

        private static async Task InsertUpdateOrDeleteGraphFindAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph with deletes and any kind of key:");
            var blog = CreateBlogAndPosts();

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                await InsertUpdateOrDeleteGraphAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            await using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.Posts[0].Title = "Post A";
                blog.Posts.Remove(blog.Posts[1]);
                blog.Posts.Add(new Post { Title = "New Post" });

                Console.WriteLine($"  Updating with URL {blog.Url}");
                await InsertUpdateOrDeleteGraphAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }
        }

        private static async Task InsertUpdateOrDeleteTrackGraphAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph using TrackGraph:");
            var blog = CreateBlogAndPosts();
            blog.IsNew = true;
            blog.Posts[0].IsNew = true;
            blog.Posts[1].IsNew = true;

            await using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                await SaveAnnotatedGraphAsync(context, blog);
            }

            await using (var context = new BloggingContext())
            {
                var read = await context.Blogs.Include(b => b.Posts).SingleAsync(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            blog.IsNew = false;
            blog.Posts[0].IsNew = false;
            blog.Posts[1].IsNew = false;

            await using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.IsChanged = true;
                blog.Posts[0].Title = "Post A";
                blog.Posts[0].IsDeleted = true;
                blog.Posts[1].Title = "Post B";
                blog.Posts.Add(new Post { Title = "New Post", IsNew = true });

                Console.WriteLine($"  Updating with URL {blog.Url}");
                await SaveAnnotatedGraphAsync(context, blog);
            }

            await using (var context = new BloggingContext())
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
        public static bool IsItNewAsync(BloggingContext context, Blog blog)
            => context.Blogs.Find(blog.BlogId) == null;
        #endregion

        #region InsertAndUpdateSingleEntity
        public static async Task InsertAsync(DbContext context, object entity)
        {
            context.Add(entity);
            await context.SaveChangesAsync();
        }

        public static async Task UpdateAsync(DbContext context, object entity)
        {
            context.Update(entity);
            await context.SaveChangesAsync();
        }
        #endregion

        #region InsertOrUpdateSingleEntity
        public static async Task InsertOrUpdateAsync(DbContext context, object entity)
        {
            context.Update(entity);
            await context.SaveChangesAsync();
        }
        #endregion

        #region InsertOrUpdateSingleEntityWithFind
        public static async Task InsertOrUpdateAsync(BloggingContext context, Blog blog)
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
                Url = "http://sample.com",
                Posts = new List<Post>
                {
                    new Post {Title = "Post 1"},
                    new Post {Title = "Post 2"},
                }
            };
            #endregion

            return blog;
        }

        #region InsertGraph
        public static async Task InsertGraphAsync(DbContext context, object rootEntity)
        {
            context.Add(rootEntity);
            await context.SaveChangesAsync();
        }
        #endregion

        #region UpdateGraph
        public static async Task UpdateGraphAsync(DbContext context, object rootEntity)
        {
            context.Update(rootEntity);
            await context.SaveChangesAsync();
        }
        #endregion

        #region InsertOrUpdateGraph
        public static async Task InsertOrUpdateGraphAsync(DbContext context, object rootEntity)
        {
            context.Update(rootEntity);
            await context.SaveChangesAsync();
        }
        #endregion

        #region InsertOrUpdateGraphWithFind
        public static async Task InsertOrUpdateGraphAsync(BloggingContext context, Blog blog)
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
                    var existingPost = await existingBlog.Posts
                        .AsQueryable()
                        .FirstOrDefaultAsync(p => p.PostId == post.PostId);

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
        public static async Task InsertUpdateOrDeleteGraphAsync(BloggingContext context, Blog blog)
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
                        .AsQueryable()
                        .FirstOrDefaultAsync(p => p.PostId == post.PostId);

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
        public static async Task SaveAnnotatedGraphAsync(DbContext context, object rootEntity)
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
}
