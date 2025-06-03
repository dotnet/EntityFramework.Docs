using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Intro;

internal class Program
{
    private static async Task Main()
    {
        using (var db = new BloggingContext())
        {
            // Remove these lines if you are running migrations from the command line
            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
        }

        #region Querying
        using (var db = new BloggingContext())
        {
            var blogs = await db.Blogs
                .Where(b => b.Rating > 3)
                .OrderBy(b => b.Url)
                .ToListAsync();
        }
        #endregion

        #region SavingData
        using (var db = new BloggingContext())
        {
            var blog = new Blog { Url = "http://sample.com" };
            db.Blogs.Add(blog);
            await db.SaveChangesAsync();
        }
        #endregion
    }
}
