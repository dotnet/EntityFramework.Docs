using System.Linq;
using BusinessLogic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFTesting.TestProject.SQLite
{
    public class BlogServiceTests
    {
        [Fact]
        public void Add_writes_to_database()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<BloggingContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new BloggingContext(options))
                {
                    EnsureCreated(context);
                }

                // Run the test against one instance of the context
                using (var context = new BloggingContext(options))
                {
                    var service = new BlogService(context);
                    service.Add("https://example.com");
                    context.SaveChanges();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new BloggingContext(options))
                {
                    Assert.Equal(1, context.Blogs.Count());
                    Assert.Equal("https://example.com", context.Blogs.Single().Url);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public void Find_searches_url()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<BloggingContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new BloggingContext(options))
                {
                    EnsureCreated(context);
                }

                // Insert seed data into the database using one instance of the context
                using (var context = new BloggingContext(options))
                {
                    context.Blogs.Add(new Blog { Url = "https://example.com/cats" });
                    context.Blogs.Add(new Blog { Url = "https://example.com/catfish" });
                    context.Blogs.Add(new Blog { Url = "https://example.com/dogs" });
                    context.SaveChanges();
                }

                // Use a clean instance of the context to run the test
                using (var context = new BloggingContext(options))
                {
                    var service = new BlogService(context);
                    var result = service.Find("cat");
                    Assert.Equal(2, result.Count());
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public void GetAllResources_returns_all_resources()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<BloggingContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new BloggingContext(options))
                {
                    EnsureCreated(context);
                }

                // Insert seed data into the database using one instance of the context
                using (var context = new BloggingContext(options))
                {
                    context.Blogs.Add(new Blog { Url = "https://example.com/cats" });
                    context.Blogs.Add(new Blog { Url = "https://example.com/catfish" });
                    context.Blogs.Add(new Blog { Url = "https://example.com/dogs" });
                    context.SaveChanges();
                }

                // Use a clean instance of the context to run the test
                using (var context = new BloggingContext(options))
                {
                    var service = new BlogService(context);
                    var result = service.GetAllResources();
                    Assert.Equal(3, result.Count());
                }
            }
            finally
            {
                connection.Close();
            }
        }

        private static void EnsureCreated(BloggingContext context)
        {
            if (context.Database.EnsureCreated())
            {
                using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                viewCommand.CommandText = @"
CREATE VIEW AllResources AS
SELECT Url
FROM Blogs;";
                viewCommand.ExecuteNonQuery();
            }
        }
    }
}
