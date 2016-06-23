using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using BusinessLogic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace TestProject
{
    [TestClass]
    public class BlogServiceTestsReadOnly
    {
        private DbContextOptions<BloggingContext> _contextOptions;

        public BlogServiceTestsReadOnly()
        {
            // Create a service provider to be shared by all test methods
            var serviceProvider = new ServiceCollection()
                 .AddEntityFrameworkInMemoryDatabase()
                 .BuildServiceProvider();

            // Create options telling the context to use an
            // InMemory database and the service provider.
            var builder = new DbContextOptionsBuilder<BloggingContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            _contextOptions = builder.Options;

            // Insert the seed data that is expected by all test methods
            using (var context = new BloggingContext(_contextOptions))
            {
                context.Blogs.Add(new Blog { Url = "http://sample.com/cats" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/catfish" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/dogs" });
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Find_with_empty_term()
        {
            using (var context = new BloggingContext(_contextOptions))
            {
                var service = new BlogService(context);
                var result = service.Find("");
                Assert.AreEqual(3, result.Count());
            }
        }

        [TestMethod]
        public void Find_with_unmatched_term()
        {
            using (var context = new BloggingContext(_contextOptions))
            {
                var service = new BlogService(context);
                var result = service.Find("horse");
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void Find_with_some_matched()
        {
            using (var context = new BloggingContext(_contextOptions))
            {
                var service = new BlogService(context);
                var result = service.Find("cat");
                Assert.AreEqual(2, result.Count());
            }
        }
    }
}
