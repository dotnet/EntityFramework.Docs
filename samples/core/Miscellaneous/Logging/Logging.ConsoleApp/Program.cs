using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace EFLogging.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Sample
            using (var db = new BloggingContext())
            {
                var serviceProvider = db.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new MyLoggerProvider());
            }
            #endregion

            using (var db = new BloggingContext())
            {
                db.Database.EnsureCreated();
                db.Blogs.Add(new Blog { Url = "http://sample.com" });
                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                foreach (var blog in db.Blogs)
                {
                    Console.WriteLine(blog.Url);
                }
            }
        }
    }
}
