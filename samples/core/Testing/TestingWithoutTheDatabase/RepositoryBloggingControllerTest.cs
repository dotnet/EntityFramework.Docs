using System.Linq;
using System.Threading.Tasks;
using EF.Testing.BloggingWebApi.Controllers;
using EF.Testing.BusinessLogic;
using Moq;
using Xunit;

namespace EF.Testing.UnitTests;

public class RepositoryBloggingControllerTest
{
    #region GetBlog
    [Fact]
    public async Task GetBlog()
    {
        // Arrange
        var repositoryMock = new Mock<IBloggingRepository>();
        repositoryMock
            .Setup(r => r.GetBlogByNameAsync("Blog2"))
            .Returns(Task.FromResult(new Blog { Name = "Blog2", Url = "http://blog2.com" }));

        var controller = new BloggingControllerWithRepository(repositoryMock.Object);

        // Act
        var blog = await controller.GetBlog("Blog2");

        // Assert
        repositoryMock.Verify(r => r.GetBlogByNameAsync("Blog2"));
        Assert.Equal("http://blog2.com", blog.Url);
    }
    #endregion

    [Fact]
    public async Task GetAllBlogs()
    {
        // Arrange
        var repositoryMock = new Mock<IBloggingRepository>();
        repositoryMock
            .Setup(r => r.GetAllBlogsAsync())
            .Returns(new[]
            {
                new Blog { Name = "Blog1", Url = "http://blog1.com" },
                new Blog { Name = "Blog2", Url = "http://blog2.com" }
            }.ToAsyncEnumerable());

        var controller = new BloggingControllerWithRepository(repositoryMock.Object);

        // Act
        var blogs = await controller.GetAllBlogs().ToListAsync();

        // Assert
        repositoryMock.Verify(r => r.GetAllBlogsAsync());
        Assert.Equal("http://blog1.com", blogs[0].Url);
        Assert.Equal("http://blog2.com", blogs[1].Url);
    }

    [Fact]
    public async Task AddBlog()
    {
        // Arrange
        var repositoryMock = new Mock<IBloggingRepository>();
        var controller = new BloggingControllerWithRepository(repositoryMock.Object);

        // Act
        await controller.AddBlog("Blog2", "http://blog2.com");

        // Assert
        repositoryMock.Verify(r => r.AddBlog(It.IsAny<Blog>()));
        repositoryMock.Verify(r => r.SaveChangesAsync());
    }

    [Fact]
    public async Task UpdateBlogUrl()
    {
        var blog = new Blog { Name = "Blog2", Url = "http://blog2.com" };

        // Arrange
        var repositoryMock = new Mock<IBloggingRepository>();
        repositoryMock
            .Setup(r => r.GetBlogByNameAsync("Blog2"))
            .Returns(Task.FromResult(blog));

        var controller = new BloggingControllerWithRepository(repositoryMock.Object);

        // Act
        await controller.UpdateBlogUrl("Blog2", "http://blog2_updated.com");

        // Assert
        repositoryMock.Verify(r => r.GetBlogByNameAsync("Blog2"));
        repositoryMock.Verify(r => r.SaveChangesAsync());
        Assert.Equal("http://blog2_updated.com", blog.Url);
    }
}
