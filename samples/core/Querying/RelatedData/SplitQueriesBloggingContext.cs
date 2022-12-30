using Microsoft.EntityFrameworkCore;

namespace EFQuerying.RelatedData;

public class SplitQueriesBloggingContext : BloggingContext
{
    #region QuerySplittingBehaviorSplitQuery
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFQuerying;Trusted_Connection=True",
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }
    #endregion
}