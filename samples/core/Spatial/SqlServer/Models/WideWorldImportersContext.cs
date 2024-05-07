using Microsoft.EntityFrameworkCore;

namespace SqlServer.Models;

// This DbContext maps to the Wide World Importers sample database which can be
// found at https://github.com/microsoft/sql-server-samples
internal class WideWorldImportersContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<StateProvince> StateProvinces { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        #region snippet_UseNetTopologySuite
        options.UseSqlServer(
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WideWorldImporters;ConnectRetryCount=0",
            x => x.UseNetTopologySuite());
    #endregion
}
