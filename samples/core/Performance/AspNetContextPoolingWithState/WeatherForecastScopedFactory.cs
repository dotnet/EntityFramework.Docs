using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Performance.AspNetContextPoolingWithState;

#region WeatherForecastScopedFactory
public class WeatherForecastScopedFactory : IDbContextFactory<WeatherForecastContext>
{
    private readonly IDbContextFactory<WeatherForecastContext> _pooledFactory;
    private readonly int _tenantId;

    public WeatherForecastScopedFactory(
        IDbContextFactory<WeatherForecastContext> pooledFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _pooledFactory = pooledFactory;

        // In this sample, we simply accept the tenant ID as a request query, which means that a client can impersonate any tenant.
        // In a real application, the tenant ID would be set based on secure authentication data.
        var tenantIdString = httpContextAccessor.HttpContext.Request.Query["TenantId"];
        if (tenantIdString != StringValues.Empty && int.TryParse(tenantIdString, out var tenantId))
        {
            _tenantId = tenantId;
        }
    }

    public WeatherForecastContext CreateDbContext()
    {
        var context = _pooledFactory.CreateDbContext();
        context.TenantId = _tenantId;
        return context;
    }
}
#endregion