using Microsoft.EntityFrameworkCore;

namespace Performance.AspNetContextPoolingWithState;

#region WeatherForecastScopedFactory
public class WeatherForecastScopedFactory : IDbContextFactory<WeatherForecastContext>
{
    const int DefaultTenantId = -1;

    readonly IDbContextFactory<WeatherForecastContext> _pooledFactory;
    readonly int _tenantId;

    public WeatherForecastScopedFactory(
        IDbContextFactory<WeatherForecastContext> pooledFactory,
        ITenant tenant)
    {
        _pooledFactory = pooledFactory;
        _tenantId = tenant?.TenantId ?? DefaultTenantId;
    }

    public WeatherForecastContext CreateDbContext()
    {
        WeatherForecastContext context = _pooledFactory.CreateDbContext();
        context.TenantId = _tenantId;
        return context;
    }
}
#endregion