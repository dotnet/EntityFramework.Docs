---
title: Entity Framework Providers - EF6
description: Entity Framework Providers in Entity Framework 6
author: SamMonoRT
ms.date: 06/27/2018
uid: ef6/fundamentals/providers/index
---

# Entity Framework 6 Providers
> [!NOTE]
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.

The Entity Framework is now being developed under an open-source license and EF6 and above will not be shipped as part of the .NET Framework. This has many advantages but also requires that EF providers be rebuilt against the EF6 assemblies. This means that EF providers for EF5 and below will not work with EF6 until they have been rebuilt.

## Which providers are available for EF6?

Providers we are aware of that have been rebuilt for EF6 include:

*   **Microsoft SQL Server provider**
    *   Built from the [Entity Framework open source code base](https://github.com/aspnet/EntityFramework6)
    *   Shipped as part of the [EntityFramework NuGet package](https://nuget.org/packages/EntityFramework)
*   **Microsoft SQL Server Compact Edition provider**
    *   Built from the [Entity Framework open source code base](https://github.com/aspnet/EntityFramework6)
    *   Shipped in the [EntityFramework.SqlServerCompact NuGet package](https://nuget.org/packages/EntityFramework.SqlServerCompact)
*   [**Devart dotConnect Data Providers**](https://www.devart.com/dotconnect/)
    *   There are third-party providers from [Devart](https://www.devart.com/) for a variety of databases including Oracle, MySQL, PostgreSQL, SQLite, Salesforce, DB2, and SQL Server
*   [**CData Software providers**](https://www.cdata.com/ado/)
    *   There are third-party providers from [CData Software](https://www.cdata.com/ado/) for a variety of data stores including Salesforce, Azure Table Storage, MySql, and many more
*   **Firebird provider**
    *   Available as a [NuGet Package](https://www.nuget.org/packages/EntityFramework.Firebird/)
*   **Visual Fox Pro provider**
    *   Available as a [NuGet package](https://www.nuget.org/packages/VFPEntityFrameworkProvider2/)
*   **MySQL**
    *   [MySQL Connector/NET for Entity Framework](https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework60.html)
*   **PostgreSQL**
    *   Npgsql is available as a [NuGet package](https://www.nuget.org/packages/EntityFramework6.Npgsql/)
*   **Oracle**
    *   ODP.NET is available as a [NuGet package](https://www.nuget.org/packages/Oracle.ManagedDataAccess.EntityFramework/)
*   **SQLite**
    *   System.Data.SQLite is available as a [NuGet package](https://www.nuget.org/packages/System.Data.SQLite/)

Note that inclusion in this list does not indicate the level of functionality or support for a given provider, only that a build for EF6 has been made available.

## Registering EF providers

Starting with Entity Framework 6 EF providers can be registered using either code-based configuration or in the application’s config file.

### Config file registration

Registration of the EF provider in app.config or web.config has the following format:


``` xml
    <entityFramework>
       <providers>
         <provider invariantName="My.Invariant.Name" type="MyProvider.MyProviderServices, MyAssembly" />
       </providers>
    </entityFramework>
```

Note that often if the EF provider is installed from NuGet, then the NuGet package will automatically add this registration to the config file. If you install the NuGet package into a project that is not the startup project for your app, then you may need to copy the registration into the config file for your startup project.

The “invariantName” in this registration is the same invariant name used to identify an ADO.NET provider. This can be found as the “invariant” attribute in a DbProviderFactories registration and as the “providerName” attribute in a connection string registration. The invariant name to use should also be included in documentation for the provider. Examples of invariant names are “System.Data.SqlClient” for SQL Server and “System.Data.SqlServerCe.4.0” for SQL Server Compact.

The “type” in this registration is the assembly-qualified name of the provider type that derives from “System.Data.Entity.Core.Common.DbProviderServices”. For example, the string to use for SQL Compact is “System.Data.Entity.SqlServerCompact.SqlCeProviderServices, EntityFramework.SqlServerCompact”. The type to use here should be included in documentation for the provider.

### Code-based registration

Starting with Entity Framework 6 application-wide configuration for EF can be specified in code. For full details see _[Entity Framework Code-Based Configuration](https://msdn.microsoft.com/data/jj680699)_. The normal way to register an EF provider using code-based configuration is to create a new class that derives from System.Data.Entity.DbConfiguration and place it in the same assembly as your DbContext class. Your DbConfiguration class should then register the provider in its constructor. For example, to register the SQL Compact provider the DbConfiguration class looks like this:

``` csharp
    public class MyConfiguration : DbConfiguration
    {
        public MyConfiguration()
        {
            SetProviderServices(
                SqlCeProviderServices.ProviderInvariantName,
                SqlCeProviderServices.Instance);
        }
    }
```

In this code “SqlCeProviderServices.ProviderInvariantName” is a convenience for the SQL Server Compact provider invariant name string (“System.Data.SqlServerCe.4.0”) and SqlCeProviderServices.Instance returns the singleton instance of the SQL Compact EF provider.

## What if the provider I need isn’t available?

If the provider is available for previous versions of EF, then we encourage you to contact the owner of the provider and ask them to create an EF6 version. You should include a reference to the [documentation for the EF6 provider model](xref:ef6/fundamentals/providers/provider-model).

## Can I write a provider myself?

It is certainly possible to create an EF provider yourself although it should not be considered a trivial undertaking. The link above about the EF6 provider model is a good place to start. You may also find it useful to use the code for the SQL Server and SQL CE provider included in the [EF open source codebase](https://github.com/aspnet/EntityFramework6) as a starting point or for reference.

Note that starting with EF6 the EF provider is less tightly coupled to the underlying ADO.NET provider. This makes it easier to write an EF provider without needing to write or wrap the ADO.NET classes.
