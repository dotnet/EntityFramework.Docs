using BusinessLogic;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace TestProject
{
    [TestClass]
    public class BlogServiceTests
    {
        private ServiceCollection _serviceCollection;
        private DbContextOptions<BloggingContext> _contextOptions;

        public BlogServiceTests()
        {
            // Create a service collection that we can create service providers from
            // A service collection defines the services that will be available in service 
            // provider instances (think of it as ServiceProviderBuilder)
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddEntityFramework().AddInMemoryDatabase();

            // Create options to tell the context to use the InMemory database
            var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();
            optionsBuilder.UseInMemoryDatabase();
            _contextOptions = optionsBuilder.Options;
        }

        [TestMethod]
        public void Add_writes_to_database()
        {
            // All contexts that share the same service provider will share the same InMemory database
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            // Run the test against one instance of the context
            using (var context = new BloggingContext(serviceProvider, _contextOptions))
            {
                var service = new BlogService(context);
                service.Add("http://sample.com");
            }

            // User a seperate instance of the context to verify correct data was saved to database
            using (var context = new BloggingContext(serviceProvider, _contextOptions))
            {
                Assert.AreEqual(1, context.Blogs.Count());
                Assert.AreEqual("http://sample.com", context.Blogs.Single().Url);
            }
        }

        [TestMethod]
        public void Find_searches_url()
        {
            // All contexts that share the same service provider will share the same InMemory database
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            // Insert seed data into the database using one instance of the context
            using (var context = new BloggingContext(serviceProvider, _contextOptions))
            {
                context.Blogs.Add(new Blog { Url = "http://sample.com/cats" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/catfish" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/dogs" });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new BloggingContext(serviceProvider, _contextOptions))
            {
                var service = new BlogService(context);
                var result = service.Find("cat");
                Assert.AreEqual(2, result.Count());
            }
        }
    }
}
