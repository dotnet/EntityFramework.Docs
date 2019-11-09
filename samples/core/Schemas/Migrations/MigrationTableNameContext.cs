using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Migrations
{
    public class MigrationTableNameContext : DbContext
    {
        #region TableNameContext
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(
                connectionString,
                x => x.MigrationsHistoryTable("__MyMigrationsHistory", "mySchema"));
        #endregion
    }
}
