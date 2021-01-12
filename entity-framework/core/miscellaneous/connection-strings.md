---
title: Connection Strings - EF Core
description: Managing connection strings under different environments with Entity Framework Core
author: bricelam
ms.date: 10/27/2016
uid: core/miscellaneous/connection-strings
---
# Connection Strings

Most database providers require some form of connection string to connect to the database. Sometimes this connection string contains sensitive information that needs to be protected. You may also need to change the connection string as you move your application between environments, such as development, testing, and production.

## ASP.NET Core

In ASP.NET Core the configuration system is very flexible, and the connection string could be stored in `appsettings.json`, an environment variable, the user secret store, or another configuration source. See the [Configuration section of the ASP.NET Core documentation](/aspnet/core/fundamentals/configuration) for more details.

For instance, you can use the [Secret Manager tool](/aspnet/core/security/app-secrets#secret-manager) to store your database password and then, in scaffolding, use a connection string that simply consists of `Name=<database-alias>`.

```dotnetcli
dotnet user-secrets set ConnectionStrings:YourDatabaseAlias "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=YourDatabase"
dotnet ef dbcontext scaffold Name=ConnectionStrings:YourDatabaseAlias Microsoft.EntityFrameworkCore.SqlServer
```

Or the following example shows the connection string stored in `appsettings.json`.

```json
{
  "ConnectionStrings": {
    "BloggingDatabase": "Server=(localdb)\\mssqllocaldb;Database=EFGetStarted.ConsoleApp.NewDb;Trusted_Connection=True;"
  },
}
```

Then the context is typically configured in `Startup.cs` with the connection string being read from configuration. Note the `GetConnectionString()` method looks for a configuration value whose key is `ConnectionStrings:<connection string name>`. You need to import the [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration) namespace to use this extension method.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<BloggingContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("BloggingDatabase")));
}
```

## WinForms & WPF Applications

WinForms, WPF, and ASP.NET 4 applications have a tried and tested connection string pattern. The connection string should be added to your application's App.config file (Web.config if you are using ASP.NET). If your connection string contains sensitive information, such as username and password, you can protect the contents of the configuration file using [Protected Configuration](/dotnet/framework/data/adonet/connection-strings-and-configuration-files#encrypting-configuration-file-sections-using-protected-configuration).

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>

  <connectionStrings>
    <add name="BloggingDatabase"
         connectionString="Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" />
  </connectionStrings>
</configuration>
```

> [!TIP]
> The `providerName` setting is not required on EF Core connection strings stored in App.config because the database provider is configured via code.

You can then read the connection string using the `ConfigurationManager` API in your context's `OnConfiguring` method. You may need to add a reference to the `System.Configuration` framework assembly to be able to use this API.

```csharp
public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["BloggingDatabase"].ConnectionString);
    }
}
```

## Universal Windows Platform (UWP)

Connection strings in a UWP application are typically a SQLite connection that just specifies a local filename. They typically do not contain sensitive information, and do not need to be changed as an application is deployed. As such, these connection strings are usually fine to be left in code, as shown below. If you wish to move them out of code then UWP supports the concept of settings, see the [App Settings section of the UWP documentation](/windows/uwp/app-settings/store-and-retrieve-app-data) for details.

```csharp
public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
            optionsBuilder.UseSqlite("Data Source=blogging.db");
    }
}
```
