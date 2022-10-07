namespace NewInEfCore7;

public static class MiscellaneousTranslationsSample
{
    public static async Task Translate_string_IndexOf()
    {
        PrintSampleName();

        await using (var context = new BlogsContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await context.Seed();
        }

        await using (var context = new BlogsContext { LoggingEnabled = true })
        {
            #region StringIndexOf
            var query = context.Posts
                .Select(post => new { post.Title, IndexOfEntity = post.Content.IndexOf("Entity") })
                .Where(post => post.IndexOfEntity > 0);
            #endregion

            await foreach (var post in query.AsAsyncEnumerable())
            {
                Console.WriteLine(post);
            }

            Console.WriteLine();
        }

        await using (var context = new BlogsContext { LoggingEnabled = true })
        {
            #region GetType
            var query = context.Posts.Where(post => post.GetType() == typeof(Post));
            #endregion

            await foreach (var post in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"{post.GetType().Name} : {post.Title}");
            }

            Console.WriteLine();
        }

        await using (var context = new BlogsContext { LoggingEnabled = true })
        {
            #region OfType
            var query = context.Posts.OfType<Post>();
            #endregion

            await foreach (var post in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"{post.GetType().Name} : {post.Title}");
            }

            Console.WriteLine();
        }

        await using (var context = new BlogsContext { LoggingEnabled = true })
        {
            #region AtTimeZone
            var query = context.Posts
                .Select(
                    post => new
                    {
                        post.Title,
                        PacificTime = EF.Functions.AtTimeZone(post.PublishedOn, "Pacific Standard Time"),
                        UkTime = EF.Functions.AtTimeZone(post.PublishedOn, "GMT Standard Time"),
                    });
            #endregion

            await foreach (var post in query.AsAsyncEnumerable())
            {
                Console.WriteLine(post);
            }

            Console.WriteLine();
        }

        await using (var context = new BlogsContext { LoggingEnabled = true })
        {
            #region FilteredInclude
            var query = context.Blogs.Include(
                blog => EF.Property<ICollection<Post>>(blog, "Posts")
                    .Where(post => post.Content.Contains(".NET"))
                    .OrderBy(post => post.Title));
            #endregion

            await foreach (var blog in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Blog {blog.Name}:");

                foreach (var post in blog.Posts)
                {
                    Console.WriteLine($"  Post {post.Title}");
                }
            }

            Console.WriteLine();
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class BlogsContext : NewInEfCore7.BlogsContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().Ignore(author => author.Contact);
            modelBuilder.Entity<Post>().Ignore(post => post.Metadata);
            base.OnModelCreating(modelBuilder);
        }
    }
}
