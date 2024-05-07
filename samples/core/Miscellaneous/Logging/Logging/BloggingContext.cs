using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EFLogging;

public class BloggingContext : DbContext
{
    #region DefineLoggerFactory
    public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(builder => { builder.AddConsole(); });
    #endregion

    public DbSet<Blog> Blogs { get; set; }

    #region RegisterLoggerFactory
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseLoggerFactory(MyLoggerFactory)
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFLogging;Trusted_Connection=True;ConnectRetryCount=0");
    #endregion
}

public class EnableSensitiveDataLoggingContext : BloggingContext
{
    #region EnableSensitiveDataLogging
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.EnableSensitiveDataLogging();
    #endregion
}

public class EnableDetailedErrorsContext : BloggingContext
{
    #region EnableDetailedErrors
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.EnableDetailedErrors();
    #endregion
}

public class ChangeLogLevelContext : BloggingContext
{
    #region ChangeLogLevel
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .ConfigureWarnings(
                b => b.Log(
                    (RelationalEventId.ConnectionOpened, LogLevel.Information),
                    (RelationalEventId.ConnectionClosed, LogLevel.Information)));
    #endregion
}

public class SuppressMessageContext : BloggingContext
{
    #region SuppressMessage
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .ConfigureWarnings(b => b.Ignore(CoreEventId.DetachedLazyLoadingWarning));
    #endregion
}

public class ThrowForEventContext : BloggingContext
{
    #region ThrowForEvent
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .ConfigureWarnings(b => b.Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning));
    #endregion
}
