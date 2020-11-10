using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main()
    {
        using (var context = new BlogsContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.AddRange(
                new Blog { Name = "Blog1" },
                new Blog { Name = "Blog2" });

            await context.SaveChangesAsync();
        }

        using (var context = new BlogsContext())
        {
            var blogs = await context.Blogs.ToListAsync();
        }
    }
}
