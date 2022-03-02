using System.Linq;
using EF.Testing.BloggingWebApi.Controllers;
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
    public void GetBlog()
    {
        using var context = Fixture.CreateContext();
        var controller = new BloggingController(context);

        var blog = controller.GetBlog("Blog2").Value;

        Assert.Equal("http://blog2.com", blog.Url);
    }
    #endregion

    #region GetAllBlogs
    [Fact]
    public void GetAllBlogs()
    {
        using var context = Fixture.CreateContext();
        var controller = new BloggingController(context);

        var blogs = controller.GetAllBlogs().Value;

        Assert.Collection(
            blogs,
            b => Assert.Equal("Blog1", b.Name),
            b => Assert.Equal("Blog2", b.Name));
    }
    #endregion

    #region AddBlog
    [Fact]
    public void AddBlog()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new BloggingController(context);
        controller.AddBlog("Blog3", "http://blog3.com");

        context.ChangeTracker.Clear();

        var blog = context.Blogs.Single(b => b.Name == "Blog3");
        Assert.Equal("http://blog3.com", blog.Url);

    }
    #endregion
}