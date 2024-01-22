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
        using BusinessLogic.BloggingContext context = TestDatabaseFixture.CreateContext();
        var controller = new BloggingController(context);

        BusinessLogic.Blog blog = controller.GetBlog("Blog2").Value;

        Assert.Equal("http://blog2.com", blog.Url);
    }
    #endregion

    #region GetAllBlogs
    [Fact]
    public void GetAllBlogs()
    {
        using BusinessLogic.BloggingContext context = TestDatabaseFixture.CreateContext();
        var controller = new BloggingController(context);

        BusinessLogic.Blog[] blogs = controller.GetAllBlogs().Value;

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
        using BusinessLogic.BloggingContext context = TestDatabaseFixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new BloggingController(context);
        controller.AddBlog("Blog3", "http://blog3.com");

        context.ChangeTracker.Clear();

        BusinessLogic.Blog blog = context.Blogs.Single(b => b.Name == "Blog3");
        Assert.Equal("http://blog3.com", blog.Url);
    }
    #endregion
}