using System.Linq;

namespace EFQuerying.Basics
{
    public class Sample
    {
        public static void Run()
        {
            #region LoadingAllData
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs.ToList();
            }
            #endregion

            #region LoadingSingleEntity
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Single(b => b.BlogId == 1);
            }
            #endregion

            #region Filtering
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Where(b => b.Url.Contains("dotnet"))
                    .ToList();
            }
            #endregion
        }
    }
}
