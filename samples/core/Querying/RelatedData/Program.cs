using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.RelatedData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            #region SingleInclude
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .ToList();
            }
            #endregion

            #region IgnoredInclude
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .Select(
                        blog => new { Id = blog.BlogId, blog.Url })
                    .ToList();
            }
            #endregion

            #region MultipleIncludes
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .Include(blog => blog.Owner)
                    .ToList();
            }
            #endregion

            #region SingleThenInclude
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .ThenInclude(post => post.Author)
                    .ToList();
            }
            #endregion

            #region MultipleThenIncludes
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .ThenInclude(post => post.Author)
                    .ThenInclude(author => author.Photo)
                    .ToList();
            }
            #endregion

            #region IncludeTree
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .ThenInclude(post => post.Author)
                    .ThenInclude(author => author.Photo)
                    .Include(blog => blog.Owner)
                    .ThenInclude(owner => owner.Photo)
                    .ToList();
            }
            #endregion

            #region MultipleLeafIncludes
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .ThenInclude(post => post.Author)
                    .Include(blog => blog.Posts)
                    .ThenInclude(post => post.Tags)
                    .ToList();
            }
            #endregion

            #region IncludeMultipleNavigationsWithSingleInclude
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Owner.AuthoredPosts)
                    .ThenInclude(post => post.Blog.Owner.Photo)
                    .ToList();
            }
            #endregion

            #region AsSplitQuery
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .AsSplitQuery()
                    .ToList();
            }
            #endregion

            using (var context = new SplitQueriesBloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            #region WithSplitQueryAsDefault
            using (var context = new SplitQueriesBloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .ToList();
            }
            #endregion

            #region AsSingleQuery
            using (var context = new SplitQueriesBloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .AsSingleQuery()
                    .ToList();
            }
            #endregion

            #region Eager
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Single(b => b.BlogId == 1);

                context.Entry(blog)
                    .Collection(b => b.Posts)
                    .Load();

                context.Entry(blog)
                    .Reference(b => b.Owner)
                    .Load();
            }
            #endregion

            #region NavQueryAggregate
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Single(b => b.BlogId == 1);

                var postCount = context.Entry(blog)
                    .Collection(b => b.Posts)
                    .Query()
                    .Count();
            }
            #endregion

            #region NavQueryFiltered
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Single(b => b.BlogId == 1);

                var goodPosts = context.Entry(blog)
                    .Collection(b => b.Posts)
                    .Query()
                    .Where(p => p.Rating > 3)
                    .ToList();
            }
            #endregion

            #region FilteredInclude
            using (var context = new BloggingContext())
            {
                var filteredBlogs = context.Blogs
                    .Include(
                        blog => blog.Posts
                            .Where(post => post.BlogId == 1)
                            .OrderByDescending(post => post.Title)
                            .Take(5))
                    .ToList();
            }
            #endregion

            #region MultipleLeafIncludesFiltered1
            using (var context = new BloggingContext())
            {
                var filteredBlogs = context.Blogs
                    .Include(blog => blog.Posts.Where(post => post.BlogId == 1))
                    .ThenInclude(post => post.Author)
                    .Include(blog => blog.Posts)
                    .ThenInclude(post => post.Tags.OrderBy(postTag => postTag.TagId).Skip(3))
                    .ToList();
            }
            #endregion

            #region MultipleLeafIncludesFiltered2
            using (var context = new BloggingContext())
            {
                var filteredBlogs = context.Blogs
                    .Include(blog => blog.Posts.Where(post => post.BlogId == 1))
                    .ThenInclude(post => post.Author)
                    .Include(blog => blog.Posts.Where(post => post.BlogId == 1))
                    .ThenInclude(post => post.Tags.OrderBy(postTag => postTag.TagId).Skip(3))
                    .ToList();
            }
            #endregion
        }
    }
}
