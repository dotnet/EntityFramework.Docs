using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class CosmosMinimalApiSample
{
    public static void Add_a_DbContext_and_provider()
    {
        Console.WriteLine($">>>> Sample: {nameof(Add_a_DbContext_and_provider)}");
        
        CosmosMinimal(null);
        CosmosNormal(null);
    }

    private static void CosmosMinimal(string[] args)
    {
        #region CosmosMinimal
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCosmos<MyDbContext>(
            "https://localhost:8081",
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
        #endregion
    }

    private static void CosmosNormal(string[] args)
    {
        #region CosmosNormal
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<MyDbContext>(
            options => options.UseCosmos(
                "https://localhost:8081",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
        #endregion
    }

    // This is a fake implementation of WebApplicationBuilder  uses to show the EF Core minimal APIs
    private class FakeWebApplicationBuilder
    {
        public ServiceCollection Services { get; } = new();
    }

    // This is a fake implementation of WebApplication uses to show the EF Core minimal APIs
    private static class WebApplication
    {
        public static FakeWebApplicationBuilder CreateBuilder(string[] args)
            => new();
    }

    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }
}
