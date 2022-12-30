using Microsoft.EntityFrameworkCore;

namespace Performance.AspNetContextPoolingWithState;

public class WeatherForecastContext : DbContext
{
    public int TenantId { get; set; }

    public WeatherForecastContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<WeatherForecast> Forecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>()
            .HasQueryFilter(f => f.TenantId == TenantId)
            .HasData(
                new WeatherForecast
                {
                    Id = 1,
                    TenantId = 0,
                    Date = new(2022, 1, 1),
                    Summary = "Chilly",
                    TemperatureC = -17
                },
                new WeatherForecast
                {
                    Id = 2,
                    TenantId = 0,
                    Date = new(2022, 1, 2),
                    Summary = "Balmy",
                    TemperatureC = 38
                },
                new WeatherForecast
                {
                    Id = 3,
                    TenantId = 1,
                    Date = new(2022, 1, 3),
                    Summary = "Sweltering",
                    TemperatureC = -7
                });
    }
}