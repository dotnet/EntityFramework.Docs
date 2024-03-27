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
        const string rootNamespace = "ConsoleApp1";
        const string outputDir = "Migrations";

        #region CustomTools
        var db = new MyDbContext();

        // Create design-time services
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkDesignTimeServices();
        serviceCollection.AddDbContextDesignTimeServices(db);
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        // Add a migration
        IMigrationsScaffolder migrationsScaffolder = serviceProvider.GetService<IMigrationsScaffolder>();
        ScaffoldedMigration migration = migrationsScaffolder.ScaffoldMigration(migrationName, rootNamespace);
        migrationsScaffolder.Save(projectDir, migration, outputDir);
        #endregion
    }
}

class MyDbContext : DbContext;
