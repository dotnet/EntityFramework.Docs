using EF.Testing.BloggingWebApi.Controllers;
using EF.Testing.BusinessLogic;
using Moq;
using Xunit;

namespace EF.Testing.UnitTests;

public class RepositoryBloggingControllerTest
{
    #region GetBlog
    [Fact]
    public void GetBlog()
    {
        // Arrange
        var repositoryMock = new Mock<IBloggingRepository>();
        repositoryMock
            .Setup(r => r.GetBlogByName("Blog2"))
            .Returns(new Blog { Name = "Blog2", Url = "http://blog2.com" });

        var controller = new BloggingControllerWithRepository(repositoryMock.Object);

        // Act
        var blog = controller.GetBlog("Blog2");

        // Assert
        repositoryMock.Verify(r => r.GetBlogByName("Blog2"));
        Assert.Equal("http://blog2.com", blog.Url);
    }
    #endregion

    [Fact]
    public void GetAllBlogs()
    {
        // Arrange
        var repositoryMock = new Mock<IBloggingRepository>();
        repositoryMock
            .Setup(r => r.GetAllBlogs())
            .Returns(new[]
            {
                new Blog { Name = "Blog1", Url = "http://blog1.com" },
                new Blog { Name = "Blog2", Url = "http://blog2.com" }
            });

        var controller = new BloggingControllerWithRepository(repositoryMock.Object);

        // Act
        var blogs = controller.GetAllBlogs().Value;

        // Assert
        repositoryMock.Verify(r => r.GetAllBlogs());
        Assert.Equal("http://blog1.com", blogs[0].Url);
        Assert.Equal("http://blog2.com", blogs[1].Url);
    }

    [Fact]
    public void AddBlog()
    {
        // Arrange
        var repositoryMock = new Mock<IBloggingRepository>();
        var controller = new BloggingControllerWithRepository(repositoryMock.Object);

        // Act
        controller.AddBlog("Blog2", "http://blog2.com");

        // Assert
        repositoryMock.Verify(r => r.AddBlog(It.IsAny<Blog>()));
        repositoryMock.Verify(r => r.SaveChanges());
    }

    [Fact]
    public void UpdateBlogUrl()
    {
        var blog = new Blog { Name = "Blog2", Url = "http://blog2.com" };

        // Arrange
        var repositoryMock = new Mock<IBloggingRepository>();
        repositoryMock
            .Setup(r => r.GetBlogByName("Blog2"))
            .Returns(blog);

        var controller = new BloggingControllerWithRepository(repositoryMock.Object);

        // Act
        controller.UpdateBlogUrl("Blog2", "http://blog2_updated.com");

        // Assert
        repositoryMock.Verify(r => r.GetBlogByName("Blog2"));
        repositoryMock.Verify(r => r.SaveChanges());
        Assert.Equal("http://blog2_updated.com", blog.Url);
    }
}