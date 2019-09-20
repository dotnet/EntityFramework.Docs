using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Disconnected
{
    public class Sample
    {
        public static void Run()
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            IsItNew();
            InsertAndUpdateSingleEntity();
            InsertOrUpdateSingleEntityStoreGenerated();
            InsertOrUpdateSingleEntityFind();
            InsertAndUpdateGraph();
            InsertOrUpdateGraphStoreGenerated();
            InsertOrUpdateGraphFind();
            InsertUpdateOrDeleteGraphFind();
            InsertUpdateOrDeleteTrackGraph();
        }

        private static void IsItNew()
        {
            Console.WriteLine();
            Console.WriteLine("Show entity-specific check for key set:");
            using (var context = new BloggingContext())
            {
                var blog = new Blog {Url = "http://sample.com"};

                // Key is not set for a new entity
                Console.WriteLine($"  Blog entity is {(IsItNew(blog) ? "new" : "existing")}.");

                context.Add(blog);
                context.SaveChanges();

                // Key is now set
                Console.WriteLine($"  Blog entity is {(IsItNew(blog) ? "new" : "existing")}.");
            }

            Console.WriteLine();
            Console.WriteLine("Show general IsKeySet:");
            using (var context = new BloggingContext())
            {
                var blog = new Blog {Url = "http://sample.com"};

                // Key is not set for a new entity
                Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");

                context.Add(blog);
                context.SaveChanges();

                // Key is now set
                Console.WriteLine($"  Blog entity is {(IsItNew(context, (object)blog) ? "new" : "existing")}.");
            }

            Console.WriteLine();
            Console.WriteLine("Show key set on Add:");
            using (var context = new BloggingContext())
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
            using (var context = new BloggingContext())
            {
                var blog = new Blog {Url = "http://sample.com"};

                Console.WriteLine($"  Blog entity is {(IsItNew(context, blog) ? "new" : "existing")}.");

                context.Add(blog);
                context.SaveChanges();

                Console.WriteLine($"  Blog entity is {(IsItNew(context, blog) ? "new" : "existing")}.");
            }
        }

        private static void InsertAndUpdateSingleEntity()
        {
            Console.WriteLine();
            Console.WriteLine("Save single entity with explicit insert or update:");
            var blog = new Blog { Url = "http://sample.com" };

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url}");
                Insert(context, blog);
            }

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }

            using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                Console.WriteLine($"  Updating with URL {blog.Url}");
                Update(context, blog);
            }

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }
        }

        private static void InsertOrUpdateSingleEntityStoreGenerated()
        {
            Console.WriteLine();
            Console.WriteLine("Save single entity with auto-generated key:");
            var blog = new Blog { Url = "http://sample.com" };

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url}");
                InsertOrUpdate(context, (object)blog);
            }

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }

            using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                Console.WriteLine($"  Updating with URL {blog.Url}");
                InsertOrUpdate(context, (object)blog);
            }

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }
        }

        private static void InsertOrUpdateSingleEntityFind()
        {
            Console.WriteLine();
            Console.WriteLine("Save single entity with any kind of key:");
            var blog = new Blog { Url = "http://sample.com" };

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url}");
                InsertOrUpdate(context, blog);
            }

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }

            using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                Console.WriteLine($"  Updating with URL {blog.Url}");
                InsertOrUpdate(context, blog);
            }

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Found with URL {context.Blogs.Single(b => b.BlogId == blog.BlogId).Url}");
            }
        }

        private static void InsertAndUpdateGraph()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph with explicit insert or update:");
            var blog = CreateBlogAndPosts();

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                InsertGraph(context, blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.Posts[0].Title = "Post A";
                blog.Posts[1].Title = "Post B";

                Console.WriteLine($"  Updating with URL {blog.Url}");
                UpdateGraph(context, blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }
        }

        private static void InsertOrUpdateGraphStoreGenerated()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph with auto-generated key:");
            var blog = CreateBlogAndPosts();

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                InsertOrUpdateGraph(context, (object)blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.Posts[0].Title = "Post A";
                blog.Posts[1].Title = "Post B";
                blog.Posts.Add(new Post { Title = "New Post" });

                Console.WriteLine($"  Updating with URL {blog.Url}");
                InsertOrUpdateGraph(context, (object)blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}, {read.Posts[2].Title}");
            }
        }

        private static void InsertOrUpdateGraphFind()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph with any kind of key:");
            var blog = CreateBlogAndPosts();

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                InsertOrUpdateGraph(context, blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.Posts[0].Title = "Post A";
                blog.Posts[1].Title = "Post B";
                blog.Posts.Add(new Post { Title = "New Post" });

                Console.WriteLine($"  Updating with URL {blog.Url}");
                InsertOrUpdateGraph(context, blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}, {read.Posts[2].Title}");
            }
        }

        private static void InsertUpdateOrDeleteGraphFind()
        {
            Console.WriteLine();
            Console.WriteLine("Save graph with deletes and any kind of key:");
            var blog = CreateBlogAndPosts();

            using (var context = new BloggingContext())
            {
                Console.WriteLine($"  Inserting with URL {blog.Url} and {blog.Posts[0].Title}, {blog.Posts[1].Title}");
                InsertUpdateOrDeleteGraph(context, blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }

            using (var context = new BloggingContext())
            {
                blog.Url = "https://sample.com";
                blog.Posts[0].Title = "Post A";
                blog.Posts.Remove(blog.Posts[1]);
                blog.Posts.Add(new Post { Title = "New Post" });

                Console.WriteLine($"  Updating with URL {blog.Url}");
                InsertUpdateOrDeleteGraph(context, blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
                Console.WriteLine($"  Found with URL {read.Url} and {read.Posts[0].Title}, {read.Posts[1].Title}");
            }
        }

        private static void InsertUpdateOrDeleteTrackGraph()
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
                SaveAnnotatedGraph(context, blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
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
                SaveAnnotatedGraph(context, blog);
            }

            using (var context = new BloggingContext())
            {
                var read = context.Blogs.Include(b => b.Posts).Single(b => b.BlogId == blog.BlogId);
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
        public static bool IsItNew(BloggingContext context, Blog blog)
            => context.Blogs.Find(blog.BlogId) == null;
        #endregion

        #region InsertAndUpdateSingleEntity
        public static void Insert(DbContext context, object entity)
        {
            context.Add(entity);
            context.SaveChanges();
        }

        public static void Update(DbContext context, object entity)
        {
            context.Update(entity);
            context.SaveChanges();
        }
        #endregion

        #region InsertOrUpdateSingleEntity
        public static void InsertOrUpdate(DbContext context, object entity)
        {
            context.Update(entity);
            context.SaveChanges();
        }
        #endregion

        #region InsertOrUpdateSingleEntityWithFind
        public static void InsertOrUpdate(BloggingContext context, Blog blog)
        {
            var existingBlog = context.Blogs.Find(blog.BlogId);
            if (existingBlog == null)
            {
                context.Add(blog);
            }
            else
            {
                context.Entry(existingBlog).CurrentValues.SetValues(blog);
            }

            context.SaveChanges();
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
        public static void InsertGraph(DbContext context, object rootEntity)
        {
            context.Add(rootEntity);
            context.SaveChanges();
        }
        #endregion

        #region UpdateGraph
        public static void UpdateGraph(DbContext context, object rootEntity)
        {
            context.Update(rootEntity);
            context.SaveChanges();
        }
        #endregion

        #region InsertOrUpdateGraph
        public static void InsertOrUpdateGraph(DbContext context, object rootEntity)
        {
            context.Update(rootEntity);
            context.SaveChanges();
        }
        #endregion

        #region InsertOrUpdateGraphWithFind
        public static void InsertOrUpdateGraph(BloggingContext context, Blog blog)
        {
            var existingBlog = context.Blogs
                .Include(b => b.Posts)
                .FirstOrDefault(b => b.BlogId == blog.BlogId);

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

            context.SaveChanges();
        }
        #endregion

        #region InsertUpdateOrDeleteGraphWithFind
        public static void InsertUpdateOrDeleteGraph(BloggingContext context, Blog blog)
        {
            var existingBlog = context.Blogs
                .Include(b => b.Posts)
                .FirstOrDefault(b => b.BlogId == blog.BlogId);

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

            context.SaveChanges();
        }
        #endregion

        #region TrackGraph
        public static void SaveAnnotatedGraph(DbContext context, object rootEntity)
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

            context.SaveChanges();
        }
        #endregion

    }
}
