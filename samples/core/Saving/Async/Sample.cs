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
            using (var context = new BloggingContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            await AddBlogAsync("http://sample.com");
        }

        #region Sample
        public static async Task AddBlogAsync(string url)
        {
            using (var context = new BloggingContext())
            {
                var blog = new Blog { Url = url };
                context.Blogs.Add(blog);
                await context.SaveChangesAsync();
            }
        }
        #endregion
    }
}
