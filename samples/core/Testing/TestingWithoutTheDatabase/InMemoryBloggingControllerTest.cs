using System.Linq;
using EF.Testing.BloggingWebApi.Controllers;
using EF.Testing.BusinessLogic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EF.Testing.UnitTests;

public class InMemoryBloggingControllerTest
{
    private readonly DbContextOptions<BloggingContext> _contextOptions;

    #region Constructor
    public InMemoryBloggingControllerTest()
    {
        _contextOptions = new DbContextOptionsBuilder<BloggingContext>()
            .UseInMemoryDatabase("BloggingControllerTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        using var context = new BloggingContext(_contextOptions);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.AddRange(
            new Blog { Name = "Blog1", Url = "http://blog1.com" },
            new Blog { Name = "Blog2", Url = "http://blog2.com" });

        context.SaveChanges();
    }
    #endregion

    #region GetBlog
    [Fact]
    public void GetBlog()
    {
        using var context = CreateContext();
        var controller = new BloggingController(context);

        var blog = controller.GetBlog("Blog2").Value;

        Assert.Equal("http://blog2.com", blog.Url);
    }
    #endregion

    [Fact]
    public void GetAllBlogs()
    {
        using var context = CreateContext();
        var controller = new BloggingController(context);

        var blogs = controller.GetAllBlogs().Value;

        Assert.Collection(
            blogs,
            b => Assert.Equal("Blog1", b.Name),
            b => Assert.Equal("Blog2", b.Name));
    }

    [Fact]
    public void AddBlog()
    {
        using var context = CreateContext();
        var controller = new BloggingController(context);

        controller.AddBlog("Blog3", "http://blog3.com");

        var blog = context.Blogs.Single(b => b.Name == "Blog3");
        Assert.Equal("http://blog3.com", blog.Url);
    }

    [Fact]
    public void UpdateBlogUrl()
    {
        using var context = CreateContext();
        var controller = new BloggingController(context);

        controller.UpdateBlogUrl("Blog2", "http://blog2_updated.com");

        var blog = context.Blogs.Single(b => b.Name == "Blog2");
        Assert.Equal("http://blog2_updated.com", blog.Url);
    }

    BloggingContext CreateContext() => new BloggingContext(_contextOptions, (context, modelBuilder) =>
    {
        #region ToInMemoryQuery
        modelBuilder.Entity<UrlResource>()
            .ToInMemoryQuery(() => context.Blogs.Select(b => new UrlResource { Url = b.Url }));
        #endregion
    });
}