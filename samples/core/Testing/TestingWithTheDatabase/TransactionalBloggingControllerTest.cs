using System;
using System.Linq;
using EF.Testing.BloggingWebApi.Controllers;
using Xunit;

namespace EF.Testing.IntegrationTests;

#region UsingTheFixture
[Collection("TransactionalTests")]
public class TransactionalBloggingControllerTest : IDisposable
{
    public TransactionalBloggingControllerTest(TransactionalTestDatabaseFixture fixture)
        => Fixture = fixture;

    public TransactionalTestDatabaseFixture Fixture { get; }
    #endregion

    #region UpdateBlogUrl
    [Fact]
    public void UpdateBlogUrl()
    {
        using (var context = Fixture.CreateContext())
        {
            var controller = new BloggingController(context);
            controller.UpdateBlogUrl("Blog2", "http://blog2_updated.com");
        }

        using (var context = Fixture.CreateContext())
        {
            var blog = context.Blogs.Single(b => b.Name == "Blog2");
            Assert.Equal("http://blog2_updated.com", blog.Url);
        }
    }
    #endregion

    #region Dispose
    public void Dispose()
        => Fixture.Cleanup();
    #endregion
}