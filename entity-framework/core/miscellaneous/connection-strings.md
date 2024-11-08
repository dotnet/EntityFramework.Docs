---
title: Connection Strings - EF Core
description: Managing connection strings under different environments with Entity Framework Core
author: SamMonoRT
ms.date: 10/27/2016
uid: core/miscellaneous/connection-strings
---
# Connection Strings

Most database providers require a connection string to connect to the database. The connection string:

* Can contain sensitive information that needs to be protected.
* May need to change when the app moves to different environments, such as development, testing, and production.

For more information, see [Secure authentication flows](/aspnet/core/security/#secure-authentication-flows)

## ASP.NET Core

The ASP.NET Core configuration can store connection strings with various providers:

* In the `appsettings.Development.json` or `appsettings.json` file.
* In an environment variable
* Using [Azure Key Vault](/azure/key-vault/keys/quick-create-net)
* Using the [Secret Manager tool](/aspnet/core/security/app-secrets#secret-manager)

> [!WARNING]
> Secrets should never be added to configuration files.

For example, the [Secret Manager tool](/aspnet/core/security/app-secrets#secret-manager) can store the database password. When scaffolding and using Secret manager, a connection string consists of `Name=<database-alias>`.

See the [Configuration section of the ASP.NET Core documentation](/aspnet/core/fundamentals/configuration) for more information.

```dotnetcli
dotnet user-secrets init
dotnet user-secrets set ConnectionStrings:YourDatabaseAlias "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=YourDatabase"
```

Then, in scaffolding, use a connection string that consists of `Name=<database-alias>`.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef dbcontext scaffold Name=ConnectionStrings:YourDatabaseAlias Microsoft.EntityFrameworkCore.SqlServer
```

### [Visual Studio PMC](#tab/vs)

```powershell
Scaffold-DbContext 'Name=ConnectionStrings:YourDatabaseAlias' Microsoft.EntityFrameworkCore.SqlServer
```

***

[!INCLUDE [managed-identities-test-non-production](~/core/includes/managed-identities-test-non-production.md)]

The following example shows the connection string stored in `appsettings.json`.

```json
{
  "ConnectionStrings": {
    "BloggingDatabase": "Server=(localdb)\\mssqllocaldb;Database=EFGetStarted.ConsoleApp.NewDb;Trusted_Connection=True;"
  },
}
```

The context is typically configured in `Program.cs` with the connection string being read from configuration. Note the [GetConnectionString](/dotnet/api/microsoft.extensions.configuration.configurationextensions.getconnectionstring) method looks for a configuration value whose key is `ConnectionStrings:<connection string name>`. `GetConnectionString` requires the [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration) namespace.

```csharp
var conString = builder.Configuration.GetConnectionString("BloggingContext") ??
     throw new InvalidOperationException("Connection string 'BloggingContext'" +
    " not found.");
builder.Services.AddDbContext<BloggingContext>(options =>
    options.UseSqlServer(conString));
```

## WinForms & WPF Applications

WinForms, WPF, and ASP.NET 4 applications have a tried and tested connection string pattern. The connection string should be added to your application's `App.config` file, or `Web.config` when using ASP.NET. Connection string containing sensitive information, such as username and password, should protect the contents of the configuration file using [Protected Configuration](/dotnet/framework/data/adonet/connection-strings-and-configuration-files#encrypting-configuration-file-sections-using-protected-configuration).

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

Connection strings in a UWP application are typically a SQLite connection that just specifies a local filename. They typically don't contain sensitive information, and don't need to be changed as an application is deployed. As such, these connection strings are usually fine to be left in code, as shown below. If you wish to move them out of code then UWP supports the concept of settings, see the [App Settings section of the UWP documentation](/windows/uwp/app-settings/store-and-retrieve-app-data) for details.

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
