using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConfigureDbContextSample;

#region BlogContext
public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options)
    {
    }

    public DbSet<Blog> Blogs => Set<Blog>();
}

public class Blog
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
}
#endregion

public class Program
{
    public static void Main(string[] args)
    {
        BasicAddDbContextExample();
        MultipleAddDbContextCallsExample();
        ConditionalConfigurationExample();
    }

    private static void BasicAddDbContextExample()
    {
        Console.WriteLine("=== Basic AddDbContext Example ===");
        
        #region BasicAddDbContext
        var services = new ServiceCollection();

        // DbContext registration with configuration
        services.AddDbContext<BlogContext>(options =>
            options.UseInMemoryDatabase("BasicExample")
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors());

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        
        Console.WriteLine($"Context configured successfully with provider: {context.Database.ProviderName}");
        Console.WriteLine();
    }

    private static void MultipleAddDbContextCallsExample()
    {
        Console.WriteLine("=== Multiple AddDbContext Calls Example (EF Core 8.0 Behavior) ===");
        
        #region MultipleAddDbContextCalls
        var services = new ServiceCollection();

        // First configuration - will be overridden in EF Core 8.0
        services.AddDbContext<BlogContext>(options =>
            options.UseInMemoryDatabase("FirstDatabase"));

        // Second configuration - this takes precedence (EF Core 8.0 behavior)
        services.AddDbContext<BlogContext>(options =>
            options.UseInMemoryDatabase("SecondDatabase")
                   .EnableSensitiveDataLogging());

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        
        Console.WriteLine($"Provider: {context.Database.ProviderName}");
        Console.WriteLine("Note: In EF Core 8.0, the second AddDbContext call takes precedence over the first one.");
        Console.WriteLine("This is different from EF Core 7.0 and earlier where the first call would win.");
        Console.WriteLine();
    }

    private static void ConditionalConfigurationExample()
    {
        Console.WriteLine("=== Conditional Configuration Example ===");
        
        #region ConditionalConfiguration
        var services = new ServiceCollection();
        
        // Mock environment and configuration services
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        
        services.AddDbContext<BlogContext>((serviceProvider, options) =>
        {
            if (isDevelopment)
            {
                options.UseInMemoryDatabase("DevelopmentDatabase")
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors();
            }
            else
            {
                options.UseInMemoryDatabase("ProductionDatabase");
            }
        });

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        
        Console.WriteLine($"Context configured for environment: {(isDevelopment ? "Development" : "Production")}");
        Console.WriteLine($"Provider: {context.Database.ProviderName}");
        Console.WriteLine();
    }
}