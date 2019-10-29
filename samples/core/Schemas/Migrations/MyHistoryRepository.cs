using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Migrations
{
    public class MyHistoryRepositoryContext : DbContext
    {
        #region HistoryRepositoryContext
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options
                .UseSqlServer(connectionString)
                .ReplaceService<IHistoryRepository, MyHistoryRepository>();
        #endregion
    }

    #region HistoryRepository
    class MyHistoryRepository : SqlServerHistoryRepository
    {
        public MyHistoryRepository(HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history.Property(h => h.MigrationId).HasColumnName("Id");
        }
    }
    #endregion
}
