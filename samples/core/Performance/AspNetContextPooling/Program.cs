using Microsoft.EntityFrameworkCore;
using Performance.AspNetContextPooling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region AddDbContextPool
builder.Services.AddDbContextPool<WeatherForecastContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("WeatherForecastContext")));
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
