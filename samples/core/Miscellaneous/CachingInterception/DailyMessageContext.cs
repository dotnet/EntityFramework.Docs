using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class DailyMessageContext : DbContext
{
    private static readonly CachingCommandInterceptor _cachingInterceptor
        = new CachingCommandInterceptor();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .EnableSensitiveDataLogging()
            .AddInterceptors(_cachingInterceptor)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .UseSqlite("DataSource=dailymessage.db");

    public DbSet<DailyMessage> DailyMessages { get; set; }
}

public class DailyMessage
{
    public int Id { get; set; }
    public string Message { get; set; }
}
