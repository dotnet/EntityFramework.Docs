using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFSaving.Async
{
    public class Sample
    {
        public static async Task RunAsync()
        {
            using (var db = new BloggingContext())
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            await AddBlogAsync("http://sample.com");
        }

        #region Sample
        public static async Task AddBlogAsync(string url)
        {
            using (var db = new BloggingContext())
            {
                var blog = new Blog { Url = url };
                db.Blogs.Add(blog);
                await db.SaveChangesAsync();
            }
        }
        #endregion
    }
}
