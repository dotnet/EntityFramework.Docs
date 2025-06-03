using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.RelatedData;

public class Sample
{
    public static async Task Run()
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        #region AddingGraphOfEntities
        using (var context = new BloggingContext())
        {
            var blog = new Blog
            {
                Url = "http://blogs.msdn.com/dotnet",
                Posts = new List<Post>
                {
                    new Post { Title = "Intro to C#" },
                    new Post { Title = "Intro to VB.NET" },
                    new Post { Title = "Intro to F#" }
                }
            };

            context.Blogs.Add(blog);
            await context.SaveChangesAsync();
        }
        #endregion

        #region AddingRelatedEntity
        using (var context = new BloggingContext())
        {
            var blog = await context.Blogs.Include(b => b.Posts).FirstAsync();
            var post = new Post { Title = "Intro to EF Core" };

            blog.Posts.Add(post);
            await context.SaveChangesAsync();
        }
        #endregion

        #region ChangingRelationships
        using (var context = new BloggingContext())
        {
            var blog = new Blog { Url = "http://blogs.msdn.com/visualstudio" };
            var post = await context.Posts.FirstAsync();

            post.Blog = blog;
            await context.SaveChangesAsync();
        }
        #endregion

        #region RemovingRelationships
        using (var context = new BloggingContext())
        {
            var blog = await context.Blogs.Include(b => b.Posts).FirstAsync();
            var post = blog.Posts.First();

            blog.Posts.Remove(post);
            await context.SaveChangesAsync();
        }
        #endregion
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFSaving.RelatedData;Trusted_Connection=True;ConnectRetryCount=0");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
