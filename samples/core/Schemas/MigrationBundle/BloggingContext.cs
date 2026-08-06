using Microsoft.EntityFrameworkCore;

namespace MigrationBundle;

public class BloggingContext(DbContextOptions<BloggingContext> options) : DbContext(options)
{
    public DbSet<Blog> Blogs => Set<Blog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSeeding(
                (context, _) =>
                {
                    if (BlogsTableExists(context)
                        && !context.Set<Blog>().Any(b => b.Url == "https://example.com"))
                    {
                        context.Set<Blog>().Add(new Blog { Url = "https://example.com" });
                        context.SaveChanges();
                    }
                })
            .UseAsyncSeeding(
                async (context, _, cancellationToken) =>
                {
                    if (await BlogsTableExistsAsync(context, cancellationToken)
                        && !await context.Set<Blog>().AnyAsync(b => b.Url == "https://example.com", cancellationToken))
                    {
                        context.Set<Blog>().Add(new Blog { Url = "https://example.com" });
                        await context.SaveChangesAsync(cancellationToken);
                    }
                });

    private static bool BlogsTableExists(DbContext context)
        => context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sqlite_master WHERE type = 'table' AND name = 'Blogs'")
            .Single() != 0;

    private static async Task<bool> BlogsTableExistsAsync(DbContext context, CancellationToken cancellationToken)
        => await context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sqlite_master WHERE type = 'table' AND name = 'Blogs'")
            .SingleAsync(cancellationToken) != 0;
}
