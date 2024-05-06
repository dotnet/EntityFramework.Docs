using EF.Testing.BusinessLogic;
using Microsoft.EntityFrameworkCore;

namespace EF.Testing.IntegrationTests;

#region TestDatabaseFixture
public class TestDatabaseFixture
{
    const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=EFTestSample;Trusted_Connection=True";

    static readonly object _lock = new();
    static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (BloggingContext context = CreateContext())
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

    public static BloggingContext CreateContext()
        => new(
            new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlServer(ConnectionString)
                .Options);
}
#endregion