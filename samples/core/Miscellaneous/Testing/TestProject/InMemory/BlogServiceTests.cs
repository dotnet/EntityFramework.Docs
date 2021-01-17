using System.Linq;
using BusinessLogic;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFTesting.TestProject.InMemory
{
    public class BlogServiceTests
    {
        [Fact]
        public void Add_writes_to_database()
        {
            var options = new DbContextOptionsBuilder<BloggingContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            // Run the test against one instance of the context
            using (var context = CreateContext(options))
            {
                var service = new BlogService(context);
                service.Add("https://example.com");
                context.SaveChanges();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = CreateContext(options))
            {
                Assert.Equal(1, context.Blogs.Count());
                Assert.Equal("https://example.com", context.Blogs.Single().Url);
            }
        }

        [Fact]
        public void Find_searches_url()
        {
            var options = new DbContextOptionsBuilder<BloggingContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = CreateContext(options))
            {
                context.Blogs.Add(new Blog { Url = "https://example.com/cats" });
                context.Blogs.Add(new Blog { Url = "https://example.com/catfish" });
                context.Blogs.Add(new Blog { Url = "https://example.com/dogs" });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = CreateContext(options))
            {
                var service = new BlogService(context);
                var result = service.Find("cat");
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public void GetAllResources_returns_all_resources()
        {
            var options = new DbContextOptionsBuilder<BloggingContext>()
                .UseInMemoryDatabase(databaseName: "GetAllResources_returns_all_resources")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = CreateContext(options))
            {
                context.Blogs.Add(new Blog { Url = "https://example.com/cats" });
                context.Blogs.Add(new Blog { Url = "https://example.com/catfish" });
                context.Blogs.Add(new Blog { Url = "https://example.com/dogs" });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = CreateContext(options))
            {
                var service = new BlogService(context);
                var result = service.GetAllResources();
                Assert.Equal(3, result.Count());
            }
        }

        private static BloggingContext CreateContext(DbContextOptions<BloggingContext> options)
            => new BloggingContext(
                options, (context, modelBuilder) =>
                {
                    modelBuilder.Entity<UrlResource>()
                        .ToInMemoryQuery(() => context.Blogs.Select(b => new UrlResource { Url = b.Url }));
                });
    }
}
