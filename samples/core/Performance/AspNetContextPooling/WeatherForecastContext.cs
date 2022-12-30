using Microsoft.EntityFrameworkCore;

namespace Performance.AspNetContextPooling;

public class WeatherForecastContext : DbContext
{
    public WeatherForecastContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<WeatherForecast> Forecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>().HasData(
            new WeatherForecast
            {
                Id = 1,
                Date = new(2022, 1, 1),
                Summary = "Chilly",
                TemperatureC = -17
            },
            new WeatherForecast
            {
                Id = 2,
                Date = new(2022, 1, 2),
                Summary = "Balmy",
                TemperatureC = 38
            },
            new WeatherForecast
            {
                Id = 3,
                Date = new(2022, 1, 3),
                Summary = "Sweltering",
                TemperatureC = -7
            });
    }
}