using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        BasicConfigureDbContextExample();
        ProviderSpecificConfigurationExample();
        ConfigurationCompositionExample();
    }

    private static void BasicConfigureDbContextExample()
    {
        Console.WriteLine("=== Basic ConfigureDbContext Example ===");
        
        #region BasicConfigureDbContext
        var services = new ServiceCollection();

        services.ConfigureDbContext<BlogContext>(options =>
            options.EnableSensitiveDataLogging()
                   .EnableDetailedErrors());

        services.AddDbContext<BlogContext>(options =>
            options.UseInMemoryDatabase("BasicExample"));

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        
        Console.WriteLine($"Context configured with provider: {context.Database.ProviderName}");
        Console.WriteLine();
    }

    private static void ProviderSpecificConfigurationExample()
    {
        Console.WriteLine("=== Provider-Specific Configuration Example ===");
        
        #region ProviderSpecificConfiguration
        var services = new ServiceCollection();

        services.ConfigureDbContext<BlogContext>(options =>
            options.UseSqlServer(sqlOptions => 
                sqlOptions.EnableRetryOnFailure()));

        services.AddDbContext<BlogContext>(options =>
            options.UseSqlServer("connectionString"));

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        Console.WriteLine("Provider-specific configuration applied");
        Console.WriteLine();
    }

    private static void ConfigurationCompositionExample()
    {
        Console.WriteLine("=== Configuration Composition Example ===");
        
        #region ConfigurationComposition
        var services = new ServiceCollection();

        services.ConfigureDbContext<BlogContext>(options =>
            options.LogTo(Console.WriteLine));

        services.AddDbContext<BlogContext>(options =>
            options.UseInMemoryDatabase("CompositionExample"));

        services.ConfigureDbContext<BlogContext>(options =>
            options.EnableSensitiveDataLogging());

        var serviceProvider = services.BuildServiceProvider();
        #endregion

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        
        Console.WriteLine($"Context configured with provider: {context.Database.ProviderName}");
        Console.WriteLine();
    }
}