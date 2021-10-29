using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.BulkConfiguration
{
    public class PreConventionContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            #region CurrencyConversion
            configurationBuilder
                .Properties<Currency>()
                .HaveConversion<CurrencyConverter>();
            #endregion

            #region StringFacets
            configurationBuilder
                .Properties<string>()
                .AreUnicode(false)
                .HaveMaxLength(1024);
            #endregion

            #region IgnoreInterface
            configurationBuilder
                .IgnoreAny(typeof(IList<>));
            #endregion

            #region DefaultTypeMapping
            configurationBuilder
                .DefaultTypeMapping<Currency>()
                .HasConversion<CurrencyConverter>();
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFModeling.BulkConfiguration;Trusted_Connection=True")
                .LogTo(Console.WriteLine, minimumLevel: Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging();
    }
}
