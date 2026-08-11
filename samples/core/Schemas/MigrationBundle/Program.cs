using Microsoft.EntityFrameworkCore;
using MigrationBundle;

if (args.Length == 2
    && args[0] == "verify")
{
    var options = new DbContextOptionsBuilder<BloggingContext>()
        .UseSqlite(args[1])
        .Options;

    await using var context = new BloggingContext(options);
    Console.WriteLine($"Blogs: {await context.Blogs.CountAsync()}");
}
