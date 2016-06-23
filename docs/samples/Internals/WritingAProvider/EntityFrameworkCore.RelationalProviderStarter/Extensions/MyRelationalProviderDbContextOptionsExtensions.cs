using EntityFrameworkCore.RelationalProviderStarter.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public static class MyRelationalProviderDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseMyRelationalProvider(this DbContextOptionsBuilder optionsBuilder,
            string connectionString)
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(
                new MyRelationalProviderOptionsExtension
                {
                    ConnectionString = connectionString
                });

            return optionsBuilder;
        }
    }
}