using Microsoft.EntityFrameworkCore;

namespace Performance;

public class BatchTweakingContext : DbContext
{
    #region BatchTweaking
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True",
            o => o
                .MinBatchSize(1)
                .MaxBatchSize(100));
    }
    #endregion
}