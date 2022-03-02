using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerService1;

internal class Program
{
    private static void Main(string[] args)
        => CreateHostBuilder(args).Build().Run();

    #region snippet_CreateHostBuilder
    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureServices(
                (hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    // Set the active provider via configuration
                    var configuration = hostContext.Configuration;
                    var provider = configuration.GetValue("Provider", "SqlServer");

                    services.AddDbContext<BlogContext>(
                        options => _ = provider switch
                        {
                            "Sqlite" => options.UseSqlite(
                                configuration.GetConnectionString("SqliteConnection"),
                                x => x.MigrationsAssembly("SqliteMigrations")),

                            "SqlServer" => options.UseSqlServer(
                                configuration.GetConnectionString("SqlServerConnection"),
                                x => x.MigrationsAssembly("SqlServerMigrations")),

                            _ => throw new Exception($"Unsupported provider: {provider}")
                        });
                });
    #endregion
}