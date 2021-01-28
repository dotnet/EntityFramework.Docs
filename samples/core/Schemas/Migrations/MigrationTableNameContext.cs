using Microsoft.EntityFrameworkCore;

namespace Migrations
{
    public class MigrationTableNameContext : DbContext
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Sample";

        #region TableNameContext
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(
                _connectionString,
                x => x.MigrationsHistoryTable("__MyMigrationsHistory", "mySchema"));
        #endregion
    }
}
