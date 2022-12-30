using Microsoft.AspNetCore.Mvc;

namespace Performance.AspNetContextPooling.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly WeatherForecastContext _dbContext;

    public WeatherForecastController(WeatherForecastContext dbContext)
        => _dbContext = dbContext;

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
        => _dbContext.Forecasts.OrderBy(f => f.Date).Take(5).ToArray();
}