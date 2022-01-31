using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Performance.AspNetContextPoolingWithState;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// First, register a pooling context factory as a Singleton service, as usual:
#region RegisterSingletonContextFactory
builder.Services.AddPooledDbContextFactory<WeatherForecastContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("WeatherForecastContext")));
#endregion

// Register an additional context factory as a Scoped service, which gets a pooled context from the Singleton factory we registered above,
// finds the tenant ID in the web request's `HttpContext`, and injects the ID into the context:
#region RegisterScopedContextFactory
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<WeatherForecastScopedFactory>();
#endregion

// Finally, arrange for a context to get injected from our Scoped factory:
#region RegisterDbContext
builder.Services.AddScoped(
    sp => sp.GetRequiredService<WeatherForecastScopedFactory>().CreateDbContext());
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
