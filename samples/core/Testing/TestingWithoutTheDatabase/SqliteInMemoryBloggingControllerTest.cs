using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using EF.Testing.BloggingWebApi.Controllers;
using EF.Testing.BusinessLogic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EF.Testing.UnitTests;

public class SqliteInMemoryBloggingControllerTest : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<BloggingContext> _contextOptions;

    #region ConstructorAndDispose
    public SqliteInMemoryBloggingControllerTest()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        _contextOptions = new DbContextOptionsBuilder<BloggingContext>()
            .UseSqlite(_connection)
            .Options;

        // Create the schema and seed some data
        using var context = new BloggingContext(_contextOptions);

        if (context.Database.EnsureCreated())
        {
            using var viewCommand = context.Database.GetDbConnection().CreateCommand();
            viewCommand.CommandText = @"
CREATE VIEW AllResources AS
SELECT Url
FROM Blogs;";
            viewCommand.ExecuteNonQuery();
        }

        context.AddRange(
            new Blog { Name = "Blog1", Url = "http://blog1.com" },
            new Blog { Name = "Blog2", Url = "http://blog2.com" });
        context.SaveChanges();
    }

    BloggingContext CreateContext() => new BloggingContext(_contextOptions);

    public void Dispose() => _connection.Dispose();
    #endregion

    #region GetBlog
    [Fact]
    public async Task GetBlog()
    {
        using var context = CreateContext();
        var controller = new BloggingController(context);

        var blog = (await controller.GetBlog("Blog2")).Value;

        Assert.Equal("http://blog2.com", blog.Url);
    }
    #endregion

    [Fact]
    public async Task GetAllBlogs()
    {
        using var context = CreateContext();
        var controller = new BloggingController(context);

        var blogs = await controller.GetAllBlogs().ToListAsync();

        Assert.Collection(
            blogs,
            b => Assert.Equal("Blog1", b.Name),
            b => Assert.Equal("Blog2", b.Name));
    }

    [Fact]
    public async Task AddBlog()
    {
        using var context = CreateContext();
        var controller = new BloggingController(context);

        await controller.AddBlog("Blog3", "http://blog3.com");

        var blog = await context.Blogs.SingleAsync(b => b.Name == "Blog3");
        Assert.Equal("http://blog3.com", blog.Url);
    }

    [Fact]
    public async Task UpdateBlogUrl()
    {
        using var context = CreateContext();
        var controller = new BloggingController(context);

        await controller.UpdateBlogUrl("Blog2", "http://blog2_updated.com");

        var blog = await context.Blogs.SingleAsync(b => b.Name == "Blog2");
        Assert.Equal("http://blog2_updated.com", blog.Url);
    }
}
