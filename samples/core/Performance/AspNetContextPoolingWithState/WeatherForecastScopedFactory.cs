using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Performance.AspNetContextPoolingWithState;

#region WeatherForecastScopedFactory
public class WeatherForecastScopedFactory : IDbContextFactory<WeatherForecastContext>
{
    private const int DefaultTenantId = -1;

    private readonly IDbContextFactory<WeatherForecastContext> _pooledFactory;
    private readonly int _tenantId;

    public WeatherForecastScopedFactory(
        IDbContextFactory<WeatherForecastContext> pooledFactory,
        ITenant tenant)
    {
        _pooledFactory = pooledFactory;
        _tenantId = tenant?.TenantId ?? DefaultTenantId;
    }

    public WeatherForecastContext CreateDbContext()
    {
        var context = _pooledFactory.CreateDbContext();
        context.TenantId = _tenantId;
        return context;
    }
}
#endregion