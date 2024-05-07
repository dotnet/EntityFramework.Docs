using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.BulkConfiguration;

public class CurrencyContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    #region ConfigureConventions
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Currency>()
            .HaveConversion<CurrencyConverter>();
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFModeling.BulkConfiguration.Currency;Trusted_Connection=True;ConnectRetryCount=0")
            .LogTo(Console.WriteLine, minimumLevel: Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging();
}
