using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace CommandLine;

public class CustomTools
{
    public static void AddMigration(string migrationName)
    {
        var projectDir = Directory.GetCurrentDirectory();
        var rootNamespace = "ConsoleApp1";
        var outputDir = "Migraitons";

        #region CustomTools
        var db = new MyDbContext();

        // Create design-time services
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkDesignTimeServices();
        serviceCollection.AddDbContextDesignTimeServices(db);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Add a migration
        var migrationsScaffolder = serviceProvider.GetService<IMigrationsScaffolder>();
        var migration = migrationsScaffolder.ScaffoldMigration(migrationName, rootNamespace);
        migrationsScaffolder.Save(projectDir, migration, outputDir);
        #endregion
    }
}

internal class MyDbContext : DbContext
{
}