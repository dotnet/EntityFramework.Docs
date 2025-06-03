using System.Linq;
using System.Threading.Tasks;
using EF.Testing.BloggingWebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EF.Testing.IntegrationTests;

#region UsingTheFixture
public class BloggingControllerTest : IClassFixture<TestDatabaseFixture>
{
    public BloggingControllerTest(TestDatabaseFixture fixture)
        => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }
    #endregion

    #region GetBlog
    [Fact]
    public async Task GetBlog()
    {
        using var context = Fixture.CreateContext();
        var controller = new BloggingController(context);

        var blog = (await controller.GetBlog("Blog2")).Value;

        Assert.Equal("http://blog2.com", blog.Url);
    }
    #endregion

    #region GetAllBlogs
    [Fact]
    public async Task GetAllBlogs()
    {
        using var context = Fixture.CreateContext();
        var controller = new BloggingController(context);

        var blogs = await controller.GetAllBlogs().ToListAsync();

        Assert.Collection(
            blogs,
            b => Assert.Equal("Blog1", b.Name),
            b => Assert.Equal("Blog2", b.Name));
    }
    #endregion

    #region AddBlog
    [Fact]
    public async Task AddBlog()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new BloggingController(context);
        await controller.AddBlog("Blog3", "http://blog3.com");

        context.ChangeTracker.Clear();

        var blog = await context.Blogs.SingleAsync(b => b.Name == "Blog3");
        Assert.Equal("http://blog3.com", blog.Url);

    }
    #endregion
}
