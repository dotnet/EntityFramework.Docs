using Microsoft.EntityFrameworkCore;

namespace SqlServer.AzureDatabase;

public class AzureSqlContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=tcp:[serverName].database.windows.net;Database=myDataBase;User ID=[Login]@[serverName];Password=myPassword];Trusted_Connection=False;Encrypt=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region HasServiceTier
        modelBuilder.HasServiceTier("BusinessCritical");
        #endregion

        #region HasDatabaseMaxSize
        modelBuilder.HasDatabaseMaxSize("2 GB");
        #endregion

        #region HasPerformanceLevelSql
        modelBuilder.HasPerformanceLevelSql("ELASTIC_POOL ( name = myelasticpool )");
        #endregion

        #region HasPerformanceLevel
        modelBuilder.HasPerformanceLevel("BC_Gen4_1");
        #endregion
    }
}