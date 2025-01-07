using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.RelatedData;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        #region SingleInclude
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .ToListAsync();
        }
        #endregion

        #region IgnoredInclude
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .Select(
                    blog => new { Id = blog.BlogId, blog.Url })
                .ToListAsync();
        }
        #endregion

        #region MultipleIncludes
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .Include(blog => blog.Owner)
                .ToListAsync();
        }
        #endregion

        #region SingleThenInclude
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .ThenInclude(post => post.Author)
                .ToListAsync();
        }
        #endregion

        #region MultipleThenIncludes
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .ThenInclude(post => post.Author)
                .ThenInclude(author => author.Photo)
                .ToListAsync();
        }
        #endregion

        #region IncludeTree
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .ThenInclude(post => post.Author)
                .ThenInclude(author => author.Photo)
                .Include(blog => blog.Owner)
                .ThenInclude(owner => owner.Photo)
                .ToListAsync();
        }
        #endregion

        #region MultipleLeafIncludes
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .ThenInclude(post => post.Author)
                .Include(blog => blog.Posts)
                .ThenInclude(post => post.Tags)
                .ToListAsync();
        }
        #endregion

        #region IncludeMultipleNavigationsWithSingleInclude
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Owner.AuthoredPosts)
                .ThenInclude(post => post.Blog.Owner.Photo)
                .ToListAsync();
        }
        #endregion

        #region AsSplitQuery
        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .AsSplitQuery()
                .ToListAsync();
        }
        #endregion

        using (var context = new SplitQueriesBloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        #region WithSplitQueryAsDefault
        using (var context = new SplitQueriesBloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .ToListAsync();
        }
        #endregion

        #region AsSingleQuery
        using (var context = new SplitQueriesBloggingContext())
        {
            var blogs = await context.Blogs
                .Include(blog => blog.Posts)
                .AsSingleQuery()
                .ToListAsync();
        }
        #endregion

        #region Explicit
        using (var context = new BloggingContext())
        {
            var blog = await context.Blogs
                .SingleAsync(b => b.BlogId == 1);

            await context.Entry(blog)
                .Collection(b => b.Posts)
                .LoadAsync();

            await context.Entry(blog)
                .Reference(b => b.Owner)
                .LoadAsync();
        }
        #endregion

        #region NavQueryAggregate
        using (var context = new BloggingContext())
        {
            var blog = await context.Blogs
                .SingleAsync(b => b.BlogId == 1);

            var postCount = await context.Entry(blog)
                .Collection(b => b.Posts)
                .Query()
                .CountAsync();
        }
        #endregion

        #region NavQueryFiltered
        using (var context = new BloggingContext())
        {
            var blog = await context.Blogs
                .SingleAsync(b => b.BlogId == 1);

            var goodPosts = await context.Entry(blog)
                .Collection(b => b.Posts)
                .Query()
                .Where(p => p.Rating > 3)
                .ToListAsync();
        }
        #endregion

        #region FilteredInclude
        using (var context = new BloggingContext())
        {
            var filteredBlogs = await context.Blogs
                .Include(
                    blog => blog.Posts
                        .Where(post => post.BlogId == 1)
                        .OrderByDescending(post => post.Title)
                        .Take(5))
                .ToListAsync();
        }
        #endregion

        #region MultipleLeafIncludesFiltered1
        using (var context = new BloggingContext())
        {
            var filteredBlogs = await context.Blogs
                .Include(blog => blog.Posts.Where(post => post.BlogId == 1))
                .ThenInclude(post => post.Author)
                .Include(blog => blog.Posts)
                .ThenInclude(post => post.Tags.OrderBy(postTag => postTag.TagId).Skip(3))
                .ToListAsync();
        }
        #endregion

        #region MultipleLeafIncludesFiltered2
        using (var context = new BloggingContext())
        {
            var filteredBlogs = await context.Blogs
                .Include(blog => blog.Posts.Where(post => post.BlogId == 1))
                .ThenInclude(post => post.Author)
                .Include(blog => blog.Posts.Where(post => post.BlogId == 1))
                .ThenInclude(post => post.Tags.OrderBy(postTag => postTag.TagId).Skip(3))
                .ToListAsync();
        }
        #endregion

        #region AutoIncludes
        using (var context = new BloggingContext())
        {
            var themes = await context.Themes.ToListAsync();
        }

        #endregion

        #region IgnoreAutoIncludes
        using (var context = new BloggingContext())
        {
            var themes = await context.Themes.IgnoreAutoIncludes().ToListAsync();
        }

        #endregion
    }
}
