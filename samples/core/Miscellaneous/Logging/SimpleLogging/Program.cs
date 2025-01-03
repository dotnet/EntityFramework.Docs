using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        await TestCode<LogToConsoleContext>();
        await TestCode<LogToDebugContext>();
        await TestCode<LogToFileContext>();
        await TestCode<InfoOnlyContext>();
        await TestCode<SensitiveDataLoggingContext>();
        await TestCode<EnableDetailedErrorsContext>();
        await TestCode<EventIdsContext>();
        await TestCode<DatabaseCategoryContext>();
        await TestCode<CustomFilterContext>();
        await TestCode<ChangeLogLevelContext>();
        await TestCode<SuppressMessageContext>();
        await TestCode<ThrowForEventContext>();
        await TestCode<UtcContext>();
        await TestCode<SingleLineContext>();
        await TestCode<TerseLogsContext>();
        await TestDatabaseLog();

        static async Task TestCode<TContext>()
            where TContext : BlogsContext, new()
        {
            using var context = new TContext();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        static async Task TestDatabaseLog()
        {
            using var context = new DatabaseLogContext();

            await context.Database.EnsureDeletedAsync();

            context.Log = Console.WriteLine;

            await context.Database.EnsureCreatedAsync();
        }
    }
}

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public abstract class BlogsContext : DbContext
{
    protected BlogsContext()
        : base(new DbContextOptionsBuilder().UseSqlite("DataSource=test.db").Options)
    {
    }

    public DbSet<Blog> Blogs { get; set; }
}

public class LogToConsoleContext : BlogsContext
{
    #region LogToConsole
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine);
    #endregion
}

public class LogToDebugContext : BlogsContext
{
    #region LogToDebug
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(message => Debug.WriteLine(message));
    #endregion
}

public class LogToFileContext : BlogsContext
{
    #region LogToFile
    private readonly StreamWriter _logStream = new StreamWriter("mylog.txt", append: true);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(_logStream.WriteLine);

    public override void Dispose()
    {
        base.Dispose();
        _logStream.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _logStream.DisposeAsync();
    }
    #endregion
}

public class InfoOnlyContext : BlogsContext
{
    #region InfoOnly
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    #endregion
}

public class SensitiveDataLoggingContext : BlogsContext
{
    #region SensitiveDataLogging
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging();
    #endregion
}

public class EnableDetailedErrorsContext : BlogsContext
{
    #region EnableDetailedErrors
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine)
            .EnableDetailedErrors();
    #endregion
}

public class EventIdsContext : BlogsContext
{
    #region EventIds
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine, new[] { CoreEventId.ContextDisposed, CoreEventId.ContextInitialized });
    #endregion
}

public class DatabaseCategoryContext : BlogsContext
{
    #region DatabaseCategory
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Name });
    #endregion
}

public class CustomFilterContext : BlogsContext
{
    #region CustomFilter
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(
                Console.WriteLine,
                (eventId, logLevel) => logLevel >= LogLevel.Information
                                       || eventId == RelationalEventId.ConnectionOpened
                                       || eventId == RelationalEventId.ConnectionClosed);
    #endregion
}

public class UtcContext : BlogsContext
{
    #region Utc
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(
            Console.WriteLine,
            LogLevel.Debug,
            DbContextLoggerOptions.DefaultWithUtcTime);
    #endregion
}

public class SingleLineContext : BlogsContext
{
    #region SingleLine
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(
            Console.WriteLine,
            LogLevel.Debug,
            DbContextLoggerOptions.DefaultWithLocalTime | DbContextLoggerOptions.SingleLine);
    #endregion
}

public class TerseLogsContext : BlogsContext
{
    #region TerseLogs
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(
            Console.WriteLine,
            LogLevel.Debug,
            DbContextLoggerOptions.UtcTime | DbContextLoggerOptions.SingleLine);
    #endregion
}

public class DatabaseLogContext : BlogsContext
{
    #region DatabaseLog
    public Action<string> Log { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(s => Log?.Invoke(s));
    #endregion
}

public class ChangeLogLevelContext : BlogsContext
{
    #region ChangeLogLevel
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .ConfigureWarnings(
                b => b.Log(
                    (RelationalEventId.ConnectionOpened, LogLevel.Information),
                    (RelationalEventId.ConnectionClosed, LogLevel.Information)))
            .LogTo(Console.WriteLine, LogLevel.Information);
    #endregion
}

public class SuppressMessageContext : BlogsContext
{
    #region SuppressMessage
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .ConfigureWarnings(b => b.Ignore(CoreEventId.DetachedLazyLoadingWarning))
            .LogTo(Console.WriteLine);
    #endregion
}

public class ThrowForEventContext : BlogsContext
{
    #region ThrowForEvent
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .ConfigureWarnings(b => b.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
            .LogTo(Console.WriteLine);
    #endregion
}
