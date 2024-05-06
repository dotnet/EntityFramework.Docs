using System;
using System.Linq;
using EF.Testing.BloggingWebApi.Controllers;
using Xunit;

namespace EF.Testing.IntegrationTests;

#region UsingTheFixture
[Collection("TransactionalTests")]
public sealed class TransactionalBloggingControllerTest : IDisposable
{
    public TransactionalBloggingControllerTest(TransactionalTestDatabaseFixture fixture)
        => Fixture = fixture;

    public TransactionalTestDatabaseFixture Fixture { get; }
    #endregion

    #region UpdateBlogUrl
    [Fact]
    public void UpdateBlogUrl()
    {
        using (BusinessLogic.BloggingContext context = TransactionalTestDatabaseFixture.CreateContext())
        {
            var controller = new BloggingController(context);
            controller.UpdateBlogUrl("Blog2", "http://blog2_updated.com");
        }

        using (BusinessLogic.BloggingContext context = TransactionalTestDatabaseFixture.CreateContext())
        {
            BusinessLogic.Blog blog = context.Blogs.Single(b => b.Name == "Blog2");
            Assert.Equal("http://blog2_updated.com", blog.Url);
        }
    }
    #endregion

    #region Dispose
    public void Dispose()
        => TransactionalTestDatabaseFixture.Cleanup();
    #endregion
}
