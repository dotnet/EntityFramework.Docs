using EF.Testing.BusinessLogic;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EF.Testing.IntegrationTests;

#region TransactionalTestDatabaseFixture
public class TransactionalTestDatabaseFixture
{
    private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=EFTransactionalTestSample;Trusted_Connection=True;ConnectRetryCount=0";

    public BloggingContext CreateContext()
        => new BloggingContext(
            new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlServer(ConnectionString)
                .Options);

    public TransactionalTestDatabaseFixture()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Cleanup();
    }

    public void Cleanup()
    {
        #region Cleanup
        using var context = CreateContext();

        context.Blogs.RemoveRange(context.Blogs);

        context.AddRange(
            new Blog { Name = "Blog1", Url = "http://blog1.com" },
            new Blog { Name = "Blog2", Url = "http://blog2.com" });
        context.SaveChanges();
        #endregion
    }
}
#endregion

#region CollectionDefinition
[CollectionDefinition("TransactionalTests")]
public class TransactionalTestsCollection : ICollectionFixture<TransactionalTestDatabaseFixture>
{
}
#endregion
