using System;
using System.Linq;
using System.Threading.Tasks;
using EF.Testing.BloggingWebApi.Controllers;
using Microsoft.EntityFrameworkCore;
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
    public async Task UpdateBlogUrl()
    {
        using (var context = Fixture.CreateContext())
        {
            var controller = new BloggingController(context);
            await controller.UpdateBlogUrl("Blog2", "http://blog2_updated.com");
        }

        using (var context = Fixture.CreateContext())
        {
            var blog = await context.Blogs.SingleAsync(b => b.Name == "Blog2");
            Assert.Equal("http://blog2_updated.com", blog.Url);
        }
    }
    #endregion

    #region Dispose
    public void Dispose()
        => Fixture.Cleanup();
    #endregion
}
