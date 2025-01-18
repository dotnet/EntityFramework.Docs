---
title: Entity Framework 6 SQL Server provider based on Microsoft.Data.SqlClient
description: Microsoft.EntityFramework.SqlServer guide
author: SamMonoRT
ms.date: 06/11/2024
uid: ef6/what-is-new/microsoft-ef6-sqlserver
---

# Entity Framework 6 SQL Server provider based on Microsoft.Data.SqlClient

This Entity Framework 6 provider is a replacement provider for the built-in SQL Server provider.

This provider depends on the modern [Microsoft.Data.SqlClient](https://github.com/dotnet/SqlClient) ADO.NET provider, which includes the following advantages over the currently used driver:

- Current client receiving full support in contrast to `System.Data.SqlClient`, which is in maintenance mode
- Supports new SQL Server features, including support for the SQL Server 2022 enhanced client protocol (TDS8)
- Supports most Azure Active Directory authentication methods
- Supports Always Encrypted with .NET

Notice that this provider is a runtime only update and will not work with the existing Visual Studio tooling.

The latest build of this package is available from [NuGet](https://www.nuget.org/packages/Microsoft.EntityFramework.SqlServer)

## Configuration

There are various ways to configure Entity Framework to use this provider.

You can register the provider in code using an attribute:

````csharp
[DbConfigurationType(typeof(MicrosoftSqlDbConfiguration))]
public class SchoolContext : DbContext
{
    public SchoolContext() : base()
    {
    }

    public DbSet<Student> Students { get; set; }
}
````

If you have multiple classes inheriting from DbContext in your solution, add the DbConfigurationType attribute to all of them.

Or you can use the SetConfiguration method before any data access calls:

````csharp
 DbConfiguration.SetConfiguration(new MicrosoftSqlDbConfiguration());
````

Or add the following lines to your existing derived DbConfiguration class:

````csharp
SetProviderFactory(MicrosoftSqlProviderServices.ProviderInvariantName, Microsoft.Data.SqlClient.SqlClientFactory.Instance);
SetProviderServices(MicrosoftSqlProviderServices.ProviderInvariantName, MicrosoftSqlProviderServices.Instance);
// Optional
SetExecutionStrategy(MicrosoftSqlProviderServices.ProviderInvariantName, () => new MicrosoftSqlAzureExecutionStrategy());
````

You can also use App.Config based configuration:

````xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <configSections>
        <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />    
    </configSections>
    <entityFramework>
        <providers>
            <provider invariantName="Microsoft.Data.SqlClient" type="System.Data.Entity.SqlServer.MicrosoftSqlProviderServices, Microsoft.EntityFramework.SqlServer" />
        </providers>
    </entityFramework>
    <system.data>
        <DbProviderFactories>
           <add name="SqlClient Data Provider"
             invariant="Microsoft.Data.SqlClient"
             description=".NET Framework Data Provider for SqlServer"
             type="Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient" />
        </DbProviderFactories>
    </system.data>
</configuration>
````

If you use App.Config with a .NET 6 or later app, you must remove the `<system.data>` section above and register the DbProviderFactory in code once:

````csharp
DbProviderFactories.RegisterFactory(MicrosoftSqlProviderServices.ProviderInvariantName, Microsoft.Data.SqlClient.SqlClientFactory.Instance);
````

## EDMX usage

If you use an EDMX file, update the `Provider` name:

````xml
<edmx:Edmx Version="3.0" xmlns:edmx="http://schemas.microsoft.com/ado/2009/11/edmx">
  <edmx:Runtime>
    <edmx:StorageModels>
      <Schema Namespace="ChinookModel.Store" Provider="Microsoft.Data.SqlClient" >
````

> In order to use the EDMX file with the Visual Studio designer, you must switch the provider name back to `System.Data.SqlClient`

Also update the provider name inside the EntityConnection connection string - `provider=Microsoft.Data.SqlClient`

````xml
 <add 
    name="Database" 
    connectionString="metadata=res://*/EFModels.csdl|res://*/EFModels.ssdl|res://*/EFModels.msl;provider=Microsoft.Data.SqlClient;provider connection string=&quot;data source=server;initial catalog=mydb;integrated security=True;persist security info=True;" 
    providerName="System.Data.EntityClient" 
 />
````

## Code changes

To use the provider in an existing solution, a few code changes are required (as needed).

`using System.Data.SqlClient;` => `using Microsoft.Data.SqlClient;`

`using Microsoft.SqlServer.Server;` => `using Microsoft.Data.SqlClient.Server;`

The following classes have been renamed to avoid conflicts with classes that uses `System.Data.SqlClient` in the existing SQL Server provider:

`SqlAzureExecutionStrategy` => `MicrosoftSqlAzureExecutionStrategy`

`SqlDbConfiguration` => `MicrosoftSqlDbConfiguration`

`SqlProviderServices` => `MicrosoftSqlProviderServices`

`SqlServerMigrationSqlGenerator` => `MicrosoftSqlServerMigrationSqlGenerator`

`SqlSpatialServices` => `MicrosoftSqlSpatialServices`

`SqlConnectionFactory` => `MicrosoftSqlConnectionFactory`

`LocalDbConnectionFactory` => `MicrosoftLocalDbConnectionFactory`

## Known issues

**Azure App Service with .NET Framework and connection strings configuration**

If you use Azure App Service with .NET Framework and the [connection strings configuration feature](/azure/app-service/configure-common?tabs=portal#configure-connection-strings), you can encounter runtime issues, as the `ProviderName` connection string setting in this scenario is hardcoded to `System.Data.SqlClient`.

Solution is to use a derived MicrosoftSqlDbConfiguration class like this:

```csharp
public class AppServiceConfiguration : MicrosoftSqlDbConfiguration
{
    public AppServiceConfiguration()
    {
        SetProviderFactory("System.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        SetProviderServices("System.Data.SqlClient", MicrosoftSqlProviderServices.Instance);
        SetExecutionStrategy("System.Data.SqlClient", () => new MicrosoftSqlAzureExecutionStrategy());
    }
}
```

Then use this derived class in the code-based configuration described above.

**EntityFramework.dll installed in GAC**

If an older version of EntityFramework.dll is installed in the .NET Framework GAC (Global Assembly Cache), you might get this error:

`The 'PrimitiveTypeKind' attribute is invalid - The value 'HierarchyId' is invalid according to its datatype`

Solution is to remove the .dll from the GAC. EF6 assemblies should never be installed in the GAC.
