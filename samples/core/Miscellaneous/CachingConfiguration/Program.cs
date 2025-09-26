using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CachingConfiguration;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== EF Core Memory Cache Configuration Examples ===\n");

        // Example 1: Service Provider Caching (Default behavior)
        ServiceProviderCachingExample();

        // Example 2: Disable Service Provider Caching
        DisableServiceProviderCachingExample();

        // Example 3: ASP.NET Core with Memory Cache
        AspNetCoreMemoryCacheExample();

        // Example 4: Performance comparison
        PerformanceComparisonExample();
    }

    static void ServiceProviderCachingExample()
    {
        Console.WriteLine("1. Service Provider Caching (Default - Enabled)");
        Console.WriteLine("================================================");

        // Create multiple contexts with same configuration
        // Service providers will be cached and reused
        var context1 = new BlogContextWithCaching();
        var context2 = new BlogContextWithCaching();

        Console.WriteLine($"Context 1 created: {context1.GetHashCode()}");
        Console.WriteLine($"Context 2 created: {context2.GetHashCode()}");
        Console.WriteLine("Service providers are cached and reused for same configuration\n");

        context1.Dispose();
        context2.Dispose();
    }

    static void DisableServiceProviderCachingExample()
    {
        Console.WriteLine("2. Service Provider Caching Disabled");
        Console.WriteLine("====================================");

        // Create contexts with caching disabled
        // Each context gets its own service provider
        var context1 = new BlogContextNoCaching();
        var context2 = new BlogContextNoCaching();

        Console.WriteLine($"Context 1 created: {context1.GetHashCode()}");
        Console.WriteLine($"Context 2 created: {context2.GetHashCode()}");
        Console.WriteLine("Each context has its own service provider (no caching)\n");

        context1.Dispose();
        context2.Dispose();
    }

    static void AspNetCoreMemoryCacheExample()
    {
        Console.WriteLine("3. ASP.NET Core Memory Cache Configuration");
        Console.WriteLine("==========================================");

        var services = new ServiceCollection();

        // Register memory cache with custom configuration
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1000;
            options.CompactionPercentage = 0.25;
        });

        // Register DbContext with explicit memory cache
        services.AddDbContext<BlogContext>(options =>
            options.UseInMemoryDatabase("TestDb")
                   .EnableServiceProviderCaching(true) // Default, shown for clarity
                   .LogTo(Console.WriteLine, LogLevel.Information));

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        Console.WriteLine($"DbContext created: {context.GetType().Name}");
        Console.WriteLine($"Memory cache available: {memoryCache != null}");
        Console.WriteLine("Memory cache is configured with size limit of 1000\n");
    }

    static void PerformanceComparisonExample()
    {
        Console.WriteLine("4. Performance Comparison");
        Console.WriteLine("=========================");

        const int iterations = 1000;

        // Test with caching enabled
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            using var context = new BlogContextWithCaching();
            // Simulate some work
            _ = context.Model;
        }
        sw.Stop();
        var cachedTime = sw.ElapsedMilliseconds;

        // Test with caching disabled
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            using var context = new BlogContextNoCaching();
            // Simulate some work
            _ = context.Model;
        }
        sw.Stop();
        var noCacheTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"Time with service provider caching: {cachedTime}ms");
        Console.WriteLine($"Time without service provider caching: {noCacheTime}ms");
        Console.WriteLine($"Performance improvement: {((double)noCacheTime / cachedTime):F1}x faster\n");
    }
}

// Context with default caching (enabled)
public class BlogContextWithCaching : DbContext
{
    public DbSet<Blog> Blogs => Set<Blog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseInMemoryDatabase("CachedDb")
            .EnableServiceProviderCaching(true); // Default behavior, explicit for clarity
    }
}

// Context with caching disabled
public class BlogContextNoCaching : DbContext
{
    public DbSet<Blog> Blogs => Set<Blog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseInMemoryDatabase("NoCacheDb")
            .EnableServiceProviderCaching(false); // Explicitly disable caching
    }
}

// Base context for DI scenarios
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
    public string Title { get; set; } = null!;
    public List<Post> Posts { get; set; } = new();
}

public class Post
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
}