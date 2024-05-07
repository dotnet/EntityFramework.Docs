using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

#pragma warning disable EF1001

namespace Migrations;

public class MyHistoryRepositoryContext : DbContext
{
    private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Sample;ConnectRetryCount=0";

    #region HistoryRepositoryContext
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseSqlServer(_connectionString)
            .ReplaceService<IHistoryRepository, MyHistoryRepository>();
    #endregion
}

#region HistoryRepository
internal class MyHistoryRepository : SqlServerHistoryRepository
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
