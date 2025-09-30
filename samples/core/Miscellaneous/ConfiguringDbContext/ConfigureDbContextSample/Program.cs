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

#region TestingExtensions
public static class TestingExtensions
{
    public static IServiceCollection AddTestingConfiguration<TContext>(
        this IServiceCollection services) 
        where TContext : DbContext
    {
        services.ConfigureDbContext<TContext>(options =>
            options.EnableSensitiveDataLogging()
                   .EnableDetailedErrors()
                   .LogTo(Console.WriteLine));
        
        return services;
    }
}
#endregion

public class Program
{
    public static void Main(string[] args)
    {
        BasicConfigureDbContextExample();
        ReusableComponentExample();
        ConfigurationCompositionExample();
    }

    private static void BasicConfigureDbContextExample()
    {
        Console.WriteLine("=== Basic ConfigureDbContext Example ===");
        
        #region BasicConfigureDbContext
        var services = new ServiceCollection();

        // Configure non-provider-specific options
        services.ConfigureDbContext<BlogContext>(options =>
            options.EnableSensitiveDataLogging()
                   .EnableDetailedErrors());

        // Add the context with provider configuration
        services.AddDbContext<BlogContext>(options =>
            options.UseInMemoryDatabase("BasicExample"));

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        
        Console.WriteLine($"Context configured with provider: {context.Database.ProviderName}");
        Console.WriteLine("ConfigureDbContext was called before AddDbContext");
        Console.WriteLine();
    }

    private static void ReusableComponentExample()
    {
        Console.WriteLine("=== Reusable Component Example ===");
        
        #region ReusableComponent
        var services = new ServiceCollection();

        // Use the testing extension method
        services.AddTestingConfiguration<BlogContext>();
        
        // Add the context with provider
        services.AddDbContext<BlogContext>(options => 
            options.UseInMemoryDatabase("TestDb"));

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        
        Console.WriteLine($"Context configured for testing with provider: {context.Database.ProviderName}");
        Console.WriteLine("Testing configuration added via extension method");
        Console.WriteLine();
    }

    private static void ConfigurationCompositionExample()
    {
        Console.WriteLine("=== Configuration Composition Example ===");
        
        #region ConfigurationComposition
        var services = new ServiceCollection();

        // First: add logging
        services.ConfigureDbContext<BlogContext>(options =>
            options.LogTo(Console.WriteLine));

        // Second: add the provider
        services.AddDbContext<BlogContext>(options =>
            options.UseInMemoryDatabase("CompositionExample"));

        // Third: add sensitive data logging
        services.ConfigureDbContext<BlogContext>(options =>
            options.EnableSensitiveDataLogging());

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        
        Console.WriteLine($"Context configured with provider: {context.Database.ProviderName}");
        Console.WriteLine("All three configurations were composed together");
        Console.WriteLine();
    }
}