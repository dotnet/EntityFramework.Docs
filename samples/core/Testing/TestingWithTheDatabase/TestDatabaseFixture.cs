using EF.Testing.BusinessLogic;
using Microsoft.EntityFrameworkCore;

namespace EF.Testing.IntegrationTests;

#region TestDatabaseFixture
public class TestDatabaseFixture
{
    private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=EFTestSample;Trusted_Connection=True;ConnectRetryCount=0";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.AddRange(
                        new Blog { Name = "Blog1", Url = "http://blog1.com" },
                        new Blog { Name = "Blog2", Url = "http://blog2.com" });
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public BloggingContext CreateContext()
        => new BloggingContext(
            new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlServer(ConnectionString)
                .Options);
}
#endregion
