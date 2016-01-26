using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using BusinessLogic;
using Microsoft.Data.Entity;
using System.Linq;
using System;

namespace EFProviders.InMemory
{
    [TestClass]
    public class BlogServiceReadOnlyTests
    {
        private IServiceProvider _serviceProvider;

        public BlogServiceReadOnlyTests()
        {
            // Create a service provider to be shared by all test methods
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<BloggingContext>(c => c.UseInMemoryDatabase());

            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Insert the seed data that is expected by all test methods
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BloggingContext>();
                context.Blogs.Add(new Blog { Url = "http://sample.com/cats" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/catfish" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/dogs" });
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Find_with_empty_term()
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BloggingContext>();
                var service = new BlogService(context);
                var result = service.Find("");
                Assert.AreEqual(3, result.Count());
            }
        }

        [TestMethod]
        public void Find_with_unmatched_term()
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BloggingContext>();
                var service = new BlogService(context);
                var result = service.Find("horse");
                Assert.AreEqual(0, result.Count());
            }
        }

        [TestMethod]
        public void Find_with_some_matched()
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BloggingContext>();
                var service = new BlogService(context);
                var result = service.Find("cat");
                Assert.AreEqual(2, result.Count());
            }
        }
    }
}
