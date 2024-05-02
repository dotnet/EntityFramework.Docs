using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class BlogsContext : DbContext
{
    private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=InterceptionTest;Trusted_Connection=True;ConnectRetryCount=0";

    private static readonly AadAuthenticationInterceptor _interceptor
        = new AadAuthenticationInterceptor();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
            .UseSqlServer(ConnectionString)
            .AddInterceptors(_interceptor);

    public DbSet<Blog> Blogs { get; set; }
}

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
}
