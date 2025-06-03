using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Performance.AspNetContextPoolingWithState.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly WeatherForecastContext _dbContext;

    public WeatherForecastController(WeatherForecastContext dbContext)
        => _dbContext = dbContext;

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
        => await _dbContext.Forecasts.OrderBy(f => f.Date).Take(5).ToArrayAsync();
}
