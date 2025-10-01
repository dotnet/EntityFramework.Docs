using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CommandLine;

public class CustomTools
{
    public static void AddMigration(string migrationName)
    {
        var projectDir = Directory.GetCurrentDirectory();
        var rootNamespace = "ConsoleApp1";
        var outputDir = "Migrations";

        #region CustomTools
        using var db = new MyDbContext();

        // Create design-time services
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContextDesignTimeServices(db);
        
        var provider = db.GetService<IDatabaseProvider>().Name;
        var providerAssembly = Assembly.Load(new AssemblyName(provider));
        var providerServicesAttribute = providerAssembly.GetCustomAttribute<DesignTimeProviderServicesAttribute>();
        var designTimeServicesType = providerAssembly.GetType(providerServicesAttribute.TypeName, throwOnError: true);
        ((IDesignTimeServices)Activator.CreateInstance(designTimeServicesType)!).ConfigureDesignTimeServices(serviceCollection);

        serviceCollection.AddEntityFrameworkDesignTimeServices();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Add a migration
        var migrationsScaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();
        var migration = migrationsScaffolder.ScaffoldMigration(migrationName, rootNamespace);
        migrationsScaffolder.Save(projectDir, migration, outputDir);
        #endregion
    }
}

internal class MyDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"Data Source=test.db");
    }

    public DbSet<Blog> Blogs { get; set; }
}

internal class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
}