# EF Core Caching Configuration Sample

This sample demonstrates various aspects of memory caching and service provider caching in Entity Framework Core.

## What This Sample Shows

1. **Service Provider Caching (Default)**: How EF Core caches internal service providers for better performance
2. **Disabled Service Provider Caching**: When and how to disable caching for specific scenarios
3. **ASP.NET Core Memory Cache Integration**: How to configure `IMemoryCache` with EF Core in dependency injection scenarios
4. **Performance Comparison**: Demonstrating the performance impact of service provider caching

## Key Concepts Demonstrated

### EnableServiceProviderCaching

- **Default**: `true` (enabled) - Service providers are cached and reused
- **When to disable**: Testing scenarios, compiled models, dynamic configurations
- **Performance impact**: Significant improvement when enabled for multiple context instances

### Memory Cache Integration

- EF Core 3.0+ requires explicit `AddMemoryCache()` registration
- `AddEntityFramework*` methods register `IMemoryCache` with size limits
- Proper configuration prevents unbounded memory growth

## Running the Sample

```bash
dotnet run
```

The sample will output performance comparisons and demonstrate different caching behaviors.

## Related Documentation

- [DbContext Configuration](../../core/dbcontext-configuration/index.md#service-provider-caching)
- [EF Core Performance](../../core/performance/index.md)
- [EF Core 3.0 Breaking Changes](../../core/what-is-new/ef-core-3.x/breaking-changes.md#addentityframework-adds-imemorycache-with-a-size-limit)