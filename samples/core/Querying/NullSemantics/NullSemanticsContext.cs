using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NullSemantics;

public class NullSemanticsContext : DbContext
{
    public DbSet<NullSemanticsEntity> Entities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var relationalNulls = false;
        if (relationalNulls)
        {
            #region UseRelationalNulls
            new SqlServerDbContextOptionsBuilder(optionsBuilder).UseRelationalNulls();
            #endregion
        }

        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=NullSemanticsSample;Trusted_Connection=True;MultipleActiveResultSets=true;ConnectRetryCount=0");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NullSemanticsEntity>().HasData(
            new NullSemanticsEntity
            {
                Id = 1,
                Int = 1,
                NullableInt = 1,
                String1 = "A",
                String2 = "A"
            },
            new NullSemanticsEntity
            {
                Id = 2,
                Int = 2,
                NullableInt = 2,
                String1 = "A",
                String2 = "B"
            },
            new NullSemanticsEntity
            {
                Id = 3,
                Int = 2,
                NullableInt = null,
                String1 = null,
                String2 = "A"
            },
            new NullSemanticsEntity
            {
                Id = 4,
                Int = 2,
                NullableInt = null,
                String1 = "B",
                String2 = null
            },
            new NullSemanticsEntity
            {
                Id = 5,
                Int = 1,
                NullableInt = 3,
                String1 = null,
                String2 = null
            });
    }
}
