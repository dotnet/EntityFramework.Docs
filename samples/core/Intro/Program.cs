using System;
using System.Linq;

namespace Intro
{
    class Program
    {
        static void Main()
        {
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
