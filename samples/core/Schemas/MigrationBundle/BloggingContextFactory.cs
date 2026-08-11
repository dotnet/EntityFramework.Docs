using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MigrationBundle;

public class BloggingContextFactory : IDesignTimeDbContextFactory<BloggingContext>
{
    public BloggingContext CreateDbContext(string[] args)
    {
        var connectionString = args.FirstOrDefault() ?? "Data Source=blogging.db";
        var options = new DbContextOptionsBuilder<BloggingContext>()
            .UseSqlite(connectionString)
            .Options;

        return new BloggingContext(options);
    }
}
