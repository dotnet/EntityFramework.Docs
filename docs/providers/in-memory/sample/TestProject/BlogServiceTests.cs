using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using BusinessLogic;
using Microsoft.Data.Entity;
using System.Linq;

namespace EFProviders.InMemory
{
    [TestClass]
    public class BlogServicTests
    {
        private ServiceCollection _serviceCollection;

        public BlogServicTests()
        {
            // Create a service collection to be shared by all test methods
            _serviceCollection = new ServiceCollection();
            _serviceCollection
                .AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<BloggingContext>(c => c.UseInMemoryDatabase());
        }

        [TestMethod]
        public void Add_writes_to_database()
        {
            // All contexts created from this service provider will access the same InMemory database
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            // Run the test against one instance of the context
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BloggingContext>();
                var service = new BlogService(context);
                service.Add("http://sample.com");
            }

            // User a seperate instance of the context to verify correct data was saved to database
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BloggingContext>();
                Assert.AreEqual(1, context.Blogs.Count());
                Assert.AreEqual("http://sample.com", context.Blogs.Single().Url);
            }
        }

        [TestMethod]
        public void Find_searches_url()
        {
            // All contexts created from this service provider will access the same InMemory database
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            // Insert seed data into the database using one instance of the context
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BloggingContext>();
                context.Blogs.Add(new Blog { Url = "http://sample.com/cats" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/catfish" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/dogs" });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BloggingContext>();
                var service = new BlogService(context);
                var result = service.Find("cat");
                Assert.AreEqual(2, result.Count());
            }
        }
    }
}
