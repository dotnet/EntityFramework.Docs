using System;

namespace ConsoleApp
{
    public class Program
    {
        public void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                Console.WriteLine("All blogs in database:");
                foreach (var blog in db.Blogs)
                {
                    Console.WriteLine(" - {0}", blog.Url);
                }
            }
        }
    }

    // TODO: Remove. Will be unnecessary when bug #2357 fixed
    // See https://github.com/aspnet/EntityFramework/issues/2357
    public class Startup
    {
        public void Configure() { }
    }
}