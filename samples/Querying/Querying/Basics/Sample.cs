using System.Linq;

namespace EFQuerying.Basics
{
    public class Sample
    {
        public static void Run()
        {
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs.ToList();
            }

            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Single(b => b.BlogId == 1);
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Where(b => b.Url.Contains("dotnet"))
                    .ToList();
            }
        }
    }
}
