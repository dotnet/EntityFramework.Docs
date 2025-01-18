---
title: Interceptors - EF Core
description: Interception for database operations and other events
author: SamMonoRT
ms.date: 11/15/2021
uid: core/logging-events-diagnostics/interceptors
---

# Interceptors

Entity Framework Core (EF Core) interceptors enable interception, modification, and/or suppression of EF Core operations. This includes low-level database operations such as executing a command, as well as higher-level operations, such as calls to SaveChanges.

Interceptors are different from logging and diagnostics in that they allow modification or suppression of the operation being intercepted. [Simple logging](xref:core/logging-events-diagnostics/simple-logging) or [Microsoft.Extensions.Logging](xref:core/logging-events-diagnostics/extensions-logging) are better choices for logging.

Interceptors are registered per DbContext instance when the context is configured. Use a [diagnostic listener](xref:core/logging-events-diagnostics/diagnostic-listeners) to get the same information but for all DbContext instances in the process.

## Registering interceptors

Interceptors are registered using <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.AddInterceptors*> when [configuring a DbContext instance](xref:core/dbcontext-configuration/index). This is commonly done in an override of <xref:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring*?displayProperty=nameWithType>. For example:

<!--
public class ExampleContext : BlogsContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new TaggedQueryCommandInterceptor());
}
-->
[!code-csharp[RegisterInterceptor](../../../samples/core/Miscellaneous/CommandInterception/Program.cs?name=RegisterInterceptor)]

Alternately, `AddInterceptors` can be called as part of <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext*> or when creating a <xref:Microsoft.EntityFrameworkCore.DbContextOptions> instance to pass to the DbContext constructor.

> [!TIP]
> OnConfiguring is still called when AddDbContext is used or a DbContextOptions instance is passed to the DbContext constructor. This makes it the ideal place to apply context configuration regardless of how the DbContext is constructed.

Interceptors are often stateless, which means that a single interceptor instance can be used for all DbContext instances. For example:

<!--
public class TaggedQueryCommandInterceptorContext : BlogsContext
{
    private static readonly TaggedQueryCommandInterceptor _interceptor
        = new TaggedQueryCommandInterceptor();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(_interceptor);
}
-->
[!code-csharp[RegisterStatelessInterceptor](../../../samples/core/Miscellaneous/CommandInterception/Program.cs?name=RegisterStatelessInterceptor)]

Every interceptor instance must implement one or more interface derived from <xref:Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor>. Each instance should only be registered once even if it implements multiple interception interfaces; EF Core will route events for each interface as appropriate.

## Database interception

> [!NOTE]
> Database interception is only available for relational database providers.

Low-level database interception is split into the three interfaces shown in the following table.

| Interceptor                                                            | Database operations intercepted
|:-----------------------------------------------------------------------|-------------------------------------------------
| <xref:Microsoft.EntityFrameworkCore.Diagnostics.IDbCommandInterceptor> | Creating commands</br>Executing commands</br>Command failures</br>Disposing the command's DbDataReader
| <xref:Microsoft.EntityFrameworkCore.Diagnostics.IDbConnectionInterceptor> | Opening and closing connections</br>Connection failures
| <xref:Microsoft.EntityFrameworkCore.Diagnostics.IDbTransactionInterceptor> | Creating transactions</br>Using existing transactions</br>Committing transactions</br>Rolling back transactions</br>Creating and using savepoints</br>Transaction failures

The base classes <xref:Microsoft.EntityFrameworkCore.Diagnostics.DbCommandInterceptor>, <xref:Microsoft.EntityFrameworkCore.Diagnostics.DbConnectionInterceptor>, and <xref:Microsoft.EntityFrameworkCore.Diagnostics.DbTransactionInterceptor> contain no-op implementations for each method in the corresponding interface. Use the base classes to avoid the need to implement unused interception methods.

The methods on each interceptor type come in pairs, with the first being called before the database operation is started, and the second after the operation has completed. For example, <xref:Microsoft.EntityFrameworkCore.Diagnostics.DbCommandInterceptor.ReaderExecuting*?displayProperty=nameWithType> is called before a query is executed, and <xref:Microsoft.EntityFrameworkCore.Diagnostics.DbCommandInterceptor.ReaderExecuted*?displayProperty=nameWithType> is called after query has been sent to the database.

Each pair of methods have both sync and async variations. This allows for asynchronous I/O, such as requesting an access token, to happen as part of intercepting an async database operation.

### Example: Command interception to add query hints

> [!TIP]
> You can [download the command interceptor sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/CommandInterception) from GitHub.

An <xref:Microsoft.EntityFrameworkCore.Diagnostics.IDbCommandInterceptor> can be used to modify SQL before it is sent to the database. This example shows how to modify the SQL to include a query hint.

Often, the trickiest part of the interception is determining when the command corresponds to the query that needs to be modified. Parsing the SQL is one option, but tends to be fragile. Another option is to use [EF Core query tags](xref:core/querying/tags) to tag each query that should be modified. For example:

<!--
            var blogs1 = context.Blogs.TagWith("Use hint: robust plan").ToList();
-->
[!code-csharp[TaggedQuery](../../../samples/core/Miscellaneous/CommandInterception/Program.cs?name=TaggedQuery)]

This tag can then be detected in the interceptor as it will always be included as a comment in the first line of the command text. On detecting the tag, the query SQL is modified to add the appropriate hint:

<!--
public class TaggedQueryCommandInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        ManipulateCommand(command);

        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        ManipulateCommand(command);

        return ValueTask.FromResult(result);
    }

    private static void ManipulateCommand(DbCommand command)
    {
        if (command.CommandText.StartsWith("-- Use hint: robust plan", StringComparison.Ordinal))
        {
            command.CommandText += " OPTION (ROBUST PLAN)";
        }
    }
}
-->
[!code-csharp[TaggedQueryCommandInterceptor](../../../samples/core/Miscellaneous/CommandInterception/TaggedQueryCommandInterceptor.cs?name=TaggedQueryCommandInterceptor)]

Notice:

* The interceptor inherits from <xref:Microsoft.EntityFrameworkCore.Diagnostics.DbCommandInterceptor> to avoid having to implement every method in the interceptor interface.
* The interceptor implements both sync and async methods. This ensures that the same query hint is applied to sync and async queries.
* The interceptor implements the `Executing` methods which are called by EF Core with the generated SQL _before_ it is sent to the database. Contrast this with the `Executed` methods, which are called after the database call has returned.

Running the code in this example generates the following when a query is tagged:

```sql
-- Use hint: robust plan

SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b] OPTION (ROBUST PLAN)
```

On the other hand, when a query is not tagged, then it is sent to the database unmodified:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
```

### Example: Connection interception for SQL Azure authentication using AAD

> [!TIP]
> You can [download the connection interceptor sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/ConnectionInterception) from GitHub.

An <xref:Microsoft.EntityFrameworkCore.Diagnostics.IDbConnectionInterceptor> can be used to manipulate the <xref:System.Data.Common.DbConnection> before it is used to connect to the database. This can be used to obtain an Azure Active Directory (AAD) access token. For example:

<!--
public class AadAuthenticationInterceptor : DbConnectionInterceptor
{
    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
        => throw new InvalidOperationException("Open connections asynchronously when using AAD authentication.");

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        var sqlConnection = (SqlConnection)connection;

        var provider = new AzureServiceTokenProvider();
        // Note: in some situations the access token may not be cached automatically the Azure Token Provider.
        // Depending on the kind of token requested, you may need to implement your own caching here.
        sqlConnection.AccessToken = await provider.GetAccessTokenAsync("https://database.windows.net/", null, cancellationToken);

        return result;
    }
}
-->
[!code-csharp[AadAuthenticationInterceptor](../../../samples/core/Miscellaneous/ConnectionInterception/AadAuthenticationInterceptor.cs?name=AadAuthenticationInterceptor)]

> [!TIP]
> [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) now supports AAD authentication via connection string. See <xref:Microsoft.Data.SqlClient.SqlAuthenticationMethod> for more information.

> [!WARNING]
> Notice that the interceptor throws if a sync call is made to open the connection. This is because there is no non-async method to obtain the access token and there is [no universal and simple way to call an async method from non-async context without risking deadlock](https://devblogs.microsoft.com/dotnet/configureawait-faq/).

> [!WARNING]
> in some situations the access token may not be cached automatically the Azure Token Provider. Depending on the kind of token requested, you may need to implement your own caching here.

### Example: Advanced command interception for caching

> [!TIP]
> You can [download the advanced command interceptor sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/CachingInterception) from GitHub.

EF Core interceptors can:

* Tell EF Core to suppress executing the operation being intercepted
* Change the result of the operation reported back to EF Core

This example shows an interceptor that uses these features to behave like a primitive second-level cache. Cached query results are returned for a specific query, avoiding a database roundtrip.

> [!WARNING]
> Take care when changing the EF Core default behavior in this way. EF Core may behave in unexpected ways if it gets an abnormal result that it cannot process correctly. Also, this example demonstrates interceptor concepts; it is not intended as a template for a robust second-level cache implementation.

In this example, the application frequently executes a query to obtain the most recent "daily message":

<!--
        async Task<string> GetDailyMessage(DailyMessageContext context)
            => (await context.DailyMessages.TagWith("Get_Daily_Message").OrderBy(e => e.Id).LastAsync()).Message;
-->
[!code-csharp[GetDailyMessage](../../../samples/core/Miscellaneous/CachingInterception/Program.cs?name=GetDailyMessage)]

This query is [tagged](xref:core/querying/tags) so that it can be easily detected in the interceptor. The idea is to only query the database for a new message once every day. At other times the application will use a cached result. (The sample uses delay of 10 seconds in the sample to simulate a new day.)

#### Interceptor state

This interceptor is stateful: it stores the ID and message text of the most recent daily message queried, plus the time when that query was executed. Because of this state we also need a [lock](/dotnet/csharp/language-reference/keywords/lock-statement) since the caching requires that same interceptor must be used by multiple context instances.

<!--
    private readonly object _lock = new object();
    private int _id;
    private string _message;
    private DateTime _queriedAt;
-->
[!code-csharp[InterceptorState](../../../samples/core/Miscellaneous/CachingInterception/CachingCommandInterceptor.cs?name=InterceptorState)]

#### Before execution

In the `Executing` method (i.e. before making a database call), the interceptor detects the tagged query and then checks if there is a cached result. If such a result is found, then the query is suppressed and cached results are used instead.

<!--
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        if (command.CommandText.StartsWith("-- Get_Daily_Message", StringComparison.Ordinal))
        {
            lock (_lock)
            {
                if (_message != null
                    && DateTime.UtcNow < _queriedAt + new TimeSpan(0, 0, 10))
                {
                    command.CommandText = "-- Get_Daily_Message: Skipping DB call; using cache.";
                    result = InterceptionResult<DbDataReader>.SuppressWithResult(new CachedDailyMessageDataReader(_id, _message));
                }
            }
        }

        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }
-->
[!code-csharp[ReaderExecutingAsync](../../../samples/core/Miscellaneous/CachingInterception/CachingCommandInterceptor.cs?name=ReaderExecutingAsync)]

Notice how the code calls <xref:Microsoft.EntityFrameworkCore.Diagnostics.InterceptionResult`1.SuppressWithResult*?displayProperty=nameWithType> and passes a replacement <xref:System.Data.Common.DbDataReader> containing the cached data. This InterceptionResult is then returned, causing suppression of query execution. The replacement reader is instead used by EF Core as the results of the query.

This interceptor also manipulates the command text. This manipulation is not required, but improves clarity in log messages. The command text does not need to be valid SQL since the query is now not going to be executed.

#### After execution

If no cached message is available, or if it has expired, then the code above does not suppress the result. EF Core will therefore execute the query as normal. It will then return to the interceptor's `Executed` method after execution. At this point if the result is not already a cached reader, then the new message ID and string is extracted from the real reader and cached ready for the next use of this query.

<!--
    public override async ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        if (command.CommandText.StartsWith("-- Get_Daily_Message", StringComparison.Ordinal)
            && !(result is CachedDailyMessageDataReader))
        {
            try
            {
                await result.ReadAsync(cancellationToken);

                lock (_lock)
                {
                    _id = result.GetInt32(0);
                    _message = result.GetString(1);
                    _queriedAt = DateTime.UtcNow;
                    return new CachedDailyMessageDataReader(_id, message);
                }
            }
            finally
            {
                await result.DisposeAsync();
            }
        }

        return result;
    }
-->
[!code-csharp[ReaderExecutedAsync](../../../samples/core/Miscellaneous/CachingInterception/CachingCommandInterceptor.cs?name=ReaderExecutedAsync)]

#### Demonstration

The [caching interceptor sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/CachingInterception) contains a simple console application that queries for daily messages to test the caching:

<!--
        // 1. Initialize the database with some daily messages.
        using (var context = new DailyMessageContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.AddRange(
                new DailyMessage { Message = "Remember: All builds are GA; no builds are RTM." },
                new DailyMessage { Message = "Keep calm and drink tea" });

            await context.SaveChangesAsync();
        }

        // 2. Query for the most recent daily message. It will be cached for 10 seconds.
        using (var context = new DailyMessageContext())
        {
            Console.WriteLine(await GetDailyMessage(context));
        }

        // 3. Insert a new daily message.
        using (var context = new DailyMessageContext())
        {
            context.Add(new DailyMessage { Message = "Free beer for unicorns" });

            await context.SaveChangesAsync();
        }

        // 4. Cached message is used until cache expires.
        using (var context = new DailyMessageContext())
        {
            Console.WriteLine(await GetDailyMessage(context));
        }

        // 5. Pretend it's the next day.
        Thread.Sleep(10000);

        // 6. Cache is expired, so the last message will noe be queried again.
        using (var context = new DailyMessageContext())
        {
            Console.WriteLine(await GetDailyMessage(context));
        }

        #region GetDailyMessage
        async Task<string> GetDailyMessage(DailyMessageContext context)
            => (await context.DailyMessages.TagWith("Get_Daily_Message").OrderBy(e => e.Id).LastAsync()).Message;
        #endregion
-->
[!code-csharp[Main](../../../samples/core/Miscellaneous/CachingInterception/Program.cs?name=Main)]

This results in the following output:

```output
info: 10/15/2020 12:32:11.801 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      -- Get_Daily_Message

      SELECT "d"."Id", "d"."Message"
      FROM "DailyMessages" AS "d"
      ORDER BY "d"."Id" DESC
      LIMIT 1

Keep calm and drink tea

info: 10/15/2020 12:32:11.821 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[@p0='Free beer for unicorns' (Size = 22)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "DailyMessages" ("Message")
      VALUES (@p0);
      SELECT "Id"
      FROM "DailyMessages"
      WHERE changes() = 1 AND "rowid" = last_insert_rowid();

info: 10/15/2020 12:32:11.826 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      -- Get_Daily_Message: Skipping DB call; using cache.

Keep calm and drink tea

info: 10/15/2020 12:32:21.833 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      -- Get_Daily_Message

      SELECT "d"."Id", "d"."Message"
      FROM "DailyMessages" AS "d"
      ORDER BY "d"."Id" DESC
      LIMIT 1

Free beer for unicorns
```

Notice from the log output that the application continues to use the cached message until the timeout expires, at which point the database is queried again for any new message.

## SaveChanges interception

> [!TIP]
> You can [download the SaveChanges interceptor sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/SaveChangesInterception) from GitHub.

<xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*> and <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*> interception points are defined by the <xref:Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor> interface. As for other interceptors, the <xref:Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor> base class with no-op methods is provided as a convenience.

> [!TIP]
> Interceptors are powerful. However, in many cases it may be easier to override the SaveChanges method or use the [.NET events for SaveChanges](xref:core/logging-events-diagnostics/events) exposed on DbContext.

### Example: SaveChanges interception for auditing

SaveChanges can be intercepted to create an independent audit record of the changes made.

> [!NOTE]
> This is not intended to be a robust auditing solution. Rather it is a simplistic example used to demonstrate the features of interception.

#### The application context

The [sample for auditing](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/SaveChangesInterception) uses a simple DbContext with blogs and posts.

<!--
public class BlogsContext : DbContext
{
    private readonly AuditingInterceptor _auditingInterceptor = new AuditingInterceptor("DataSource=audit.db");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .AddInterceptors(_auditingInterceptor)
            .UseSqlite("DataSource=blogs.db");

    public DbSet<Blog> Blogs { get; set; }
}

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Post> Posts { get; } = new List<Post>();
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }

    public Blog Blog { get; set; }
}
-->
[!code-csharp[BlogsContext](../../../samples/core/Miscellaneous/SaveChangesInterception/BlogsContext.cs?name=BlogsContext)]

Notice that a new instance of the interceptor is registered for each DbContext instance. This is because the auditing interceptor contains state linked to the current context instance.

#### The audit context

The sample also contains a second DbContext and model used for the auditing database.

<!--
public class AuditContext : DbContext
{
    private readonly string _connectionString;

    public AuditContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(_connectionString);

    public DbSet<SaveChangesAudit> SaveChangesAudits { get; set; }
}

public class SaveChangesAudit
{
    public int Id { get; set; }
    public Guid AuditId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Succeeded { get; set; }
    public string ErrorMessage { get; set; }

    public ICollection<EntityAudit> Entities { get; } = new List<EntityAudit>();
}

public class EntityAudit
{
    public int Id { get; set; }
    public EntityState State { get; set; }
    public string AuditMessage { get; set; }

    public SaveChangesAudit SaveChangesAudit { get; set; }
}
-->
[!code-csharp[AuditContext](../../../samples/core/Miscellaneous/SaveChangesInterception/AuditContext.cs?name=AuditContext)]

#### The interceptor

The general idea for auditing with the interceptor is:

* An audit message is created at the beginning of SaveChanges and is written to the auditing database
* SaveChanges is allowed to continue
* If SaveChanges succeeds, then the audit message is updated to indicate success
* If SaveChanges fails, then the audit message is updated to indicate the failure

The first stage is handled before any changes are sent to the database using overrides of <xref:Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor.SavingChanges*?displayProperty=nameWithType> and <xref:Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor.SavingChangesAsync*?displayProperty=nameWithType>.

<!--
    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _audit = CreateAudit(eventData.Context);

        using (var auditContext = new AuditContext(_connectionString))
        {
            auditContext.Add(_audit);
            await auditContext.SaveChangesAsync();
        }

        return result;
    }

    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        _audit = CreateAudit(eventData.Context);

        using (var auditContext = new AuditContext(_connectionString))
        {
            auditContext.Add(_audit);
            auditContext.SaveChanges();
        }

        return result;
    }
-->
[!code-csharp[SavingChanges](../../../samples/core/Miscellaneous/SaveChangesInterception/AuditingInterceptor.cs?name=SavingChanges)]

Overriding both sync and async methods ensures that auditing will happen regardless of whether `SaveChanges` or `SaveChangesAsync` are called. Notice also that the async overload is itself able to perform non-blocking async I/O to the auditing database. You may wish to throw from the sync `SavingChanges` method to ensure that all database I/O is async. This then requires that the application always calls `SaveChangesAsync` and never `SaveChanges`.

#### The audit message

Every interceptor method has an `eventData` parameter providing contextual information about the event being intercepted. In this case the current application DbContext is included in the event data, which is then used to create an audit message.

<!--
    private static SaveChangesAudit CreateAudit(DbContext context)
    {
        context.ChangeTracker.DetectChanges();

        var audit = new SaveChangesAudit
        {
            AuditId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow
        };

        foreach (var entry in context.ChangeTracker.Entries())
        {
            var auditMessage = entry.State switch
            {
                EntityState.Deleted => CreateDeletedMessage(entry),
                EntityState.Modified => CreateModifiedMessage(entry),
                EntityState.Added => CreateAddedMessage(entry),
                _ => null
            };

            if (auditMessage != null)
            {
                audit.Entities.Add(new EntityAudit { State = entry.State, AuditMessage = auditMessage });
            }
        }

        return audit;

        string CreateAddedMessage(EntityEntry entry)
            => entry.Properties.Aggregate(
                $"Inserting {entry.Metadata.DisplayName()} with ",
                (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");

        string CreateModifiedMessage(EntityEntry entry)
            => entry.Properties.Where(property => property.IsModified || property.Metadata.IsPrimaryKey()).Aggregate(
                $"Updating {entry.Metadata.DisplayName()} with ",
                (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");

        string CreateDeletedMessage(EntityEntry entry)
            => entry.Properties.Where(property => property.Metadata.IsPrimaryKey()).Aggregate(
                $"Deleting {entry.Metadata.DisplayName()} with ",
                (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' ");
    }
-->
[!code-csharp[CreateAudit](../../../samples/core/Miscellaneous/SaveChangesInterception/AuditingInterceptor.cs?name=CreateAudit)]

The result is a `SaveChangesAudit` entity with a collection of `EntityAudit` entities, one for each insert, update, or delete. The interceptor then inserts these entities into the audit database.

> [!TIP]
> ToString is overridden in every EF Core event data class to generate the equivalent log message for the event. For example, calling `ContextInitializedEventData.ToString` generates "Entity Framework Core 5.0.0 initialized 'BlogsContext' using provider 'Microsoft.EntityFrameworkCore.Sqlite' with options: None".

#### Detecting success

The audit entity is stored on the interceptor so that it can be accessed again once SaveChanges either succeeds or fails. For success, <xref:Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor.SavedChanges*?displayProperty=nameWithType> or <xref:Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor.SavedChangesAsync*?displayProperty=nameWithType> is called.

<!--
    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        using (var auditContext = new AuditContext(_connectionString))
        {
            auditContext.Attach(_audit);
            _audit.Succeeded = true;
            _audit.EndTime = DateTime.UtcNow;

            auditContext.SaveChanges();
        }

        return result;
    }

    public async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        using (var auditContext = new AuditContext(_connectionString))
        {
            auditContext.Attach(_audit);
            _audit.Succeeded = true;
            _audit.EndTime = DateTime.UtcNow;

            await auditContext.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
-->
[!code-csharp[SavedChanges](../../../samples/core/Miscellaneous/SaveChangesInterception/AuditingInterceptor.cs?name=SavedChanges)]

The audit entity is attached to the audit context, since it already exists in the database and needs to be updated. We then set `Succeeded` and `EndTime`, which marks these properties as modified so SaveChanges will send an update to the audit database.

#### Detecting failure

Failure is handled in much the same way as success, but in the <xref:Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor.SaveChangesFailed*?displayProperty=nameWithType> or <xref:Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor.SaveChangesFailedAsync*?displayProperty=nameWithType> method. The event data contains the exception that was thrown.

<!--
    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        using (var auditContext = new AuditContext(_connectionString))
        {
            auditContext.Attach(_audit);
            _audit.Succeeded = false;
            _audit.EndTime = DateTime.UtcNow;
            _audit.ErrorMessage = eventData.Exception.Message;

            auditContext.SaveChanges();
        }
    }

    public async Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = new CancellationToken())
    {
        using (var auditContext = new AuditContext(_connectionString))
        {
            auditContext.Attach(_audit);
            _audit.Succeeded = false;
            _audit.EndTime = DateTime.UtcNow;
            _audit.ErrorMessage = eventData.Exception.InnerException?.Message;

            await auditContext.SaveChangesAsync(cancellationToken);
        }
    }
-->
[!code-csharp[SaveChangesFailed](../../../samples/core/Miscellaneous/SaveChangesInterception/AuditingInterceptor.cs?name=SaveChangesFailed)]

#### Demonstration

The [auditing sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/SaveChangesInterception) contains a simple console application that makes changes to the blogging database and then shows the auditing that was created.

<!--
        // Insert, update, and delete some entities

        using (var context = new BlogsContext())
        {
            context.Add(
                new Blog
                {
                    Name = "EF Blog",
                    Posts =
                    {
                        new Post { Title = "EF Core 3.1!" },
                        new Post { Title = "EF Core 5.0!" }
                    }
                });

            await context.SaveChangesAsync();
        }

        using (var context = new BlogsContext())
        {
            var blog = context.Blogs.Include(e => e.Posts).Single();

            blog.Name = "EF Core Blog";
            context.Remove(blog.Posts.First());
            blog.Posts.Add(new Post { Title = "EF Core 6.0!" });

            context.SaveChanges();
        }

        // Do an insert that will fail

        using (var context = new BlogsContext())
        {
            try
            {
                context.Add(new Post { Id = 3, Title = "EF Core 3.1!" });

                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
            }
        }

        // Look at the audit trail

        using (var context = new AuditContext("DataSource=audit.db"))
        {
            foreach (var audit in context.SaveChangesAudits.Include(e => e.Entities).ToList())
            {
                Console.WriteLine(
                    $"Audit {audit.AuditId} from {audit.StartTime} to {audit.EndTime} was{(audit.Succeeded ? "" : " not")} successful.");

                foreach (var entity in audit.Entities)
                {
                    Console.WriteLine($"  {entity.AuditMessage}");
                }

                if (!audit.Succeeded)
                {
                    Console.WriteLine($"  Error: {audit.ErrorMessage}");
                }
            }
        }
-->
[!code-csharp[Program](../../../samples/core/Miscellaneous/SaveChangesInterception/Program.cs?name=Program)]

The result shows the contents of the auditing database:

```output
Audit 52e94327-1767-4046-a3ca-4c6b1eecbca6 from 10/14/2020 9:10:17 PM to 10/14/2020 9:10:17 PM was successful.
  Inserting Blog with Id: '-2147482647' Name: 'EF Blog'
  Inserting Post with Id: '-2147482647' BlogId: '-2147482647' Title: 'EF Core 3.1!'
  Inserting Post with Id: '-2147482646' BlogId: '-2147482647' Title: 'EF Core 5.0!'
Audit 8450f57a-5030-4211-a534-eb66b8da7040 from 10/14/2020 9:10:17 PM to 10/14/2020 9:10:17 PM was successful.
  Inserting Post with Id: '-2147482645' BlogId: '1' Title: 'EF Core 6.0!'
  Updating Blog with Id: '1' Name: 'EF Core Blog'
  Deleting Post with Id: '1'
Audit 201fef4d-66a7-43ad-b9b6-b57e9d3f37b3 from 10/14/2020 9:10:17 PM to 10/14/2020 9:10:17 PM was not successful.
  Inserting Post with Id: '3' BlogId: '' Title: 'EF Core 3.1!'
  Error: SQLite Error 19: 'UNIQUE constraint failed: Post.Id'.
```
