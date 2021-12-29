using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SqlServer.Plugin;

public class CustomPlugin
{
    public static void Run()
    {
        using (var context = new BloggingContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        using (var context = new BloggingContext())
        {
            context.Blogs.Add(new Blog { BlogId = 100, Url = "http://blog1.somesite.com" });

            context.SaveChanges();

            var simple = context.Blogs.Select(b => EF.Functions.Augment(b.BlogId)).First();
            var withCast = context.Blogs.Select(b => EF.Functions.Augment((long)b.BlogId)).First();
        }
    }
}
