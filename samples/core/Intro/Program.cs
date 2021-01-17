using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Intro
{
    internal class Program
    {
        private static void Main()
        {
            using (var db = new BloggingContext())
            {
                // Remove these lines if you are running migrations from the command line
                db.Database.EnsureDeleted();
                db.Database.Migrate();
            }

            #region Querying
            using (var db = new BloggingContext())
            {
                var blogs = db.Blogs
                    .Where(b => b.Rating > 3)
                    .OrderBy(b => b.Url)
                    .ToList();
            }
            #endregion

            #region SavingData
            using (var db = new BloggingContext())
            {
                var blog = new Blog { Url = "http://sample.com" };
                db.Blogs.Add(blog);
                db.SaveChanges();
            }
            #endregion
        }
    }
}
