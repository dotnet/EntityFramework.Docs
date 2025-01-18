---
title: Dependency resolution - EF6
description: Dependency resolution in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/configuring/dependency-resolution
---
# Dependency resolution
> [!NOTE]
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.  

Starting with EF6, Entity Framework contains a general-purpose mechanism for obtaining implementations of services that it requires. That is, when EF uses an instance of some interfaces or base classes it will ask for a concrete implementation of the interface or base class to use. This is achieved through use of the IDbDependencyResolver interface:  

``` csharp
public interface IDbDependencyResolver
{
    object GetService(Type type, object key);
}
```  

The GetService method is typically called by EF and is handled by an implementation of IDbDependencyResolver provided either by EF or by the application. When called, the type argument is the interface or base class type of the service being requested, and the key object is either null or an object providing contextual information about the requested service.  

Unless otherwise stated any object returned must be thread-safe since it can be used as a singleton. In many cases the object returned is a factory in which case the factory itself must be thread-safe but the object returned from the factory does not need to be thread-safe since a new instance is requested from the factory for each use.

This article does not contain full details on how to implement IDbDependencyResolver, but instead acts as a reference for the service types (that is, the interface and base class types) for which EF calls GetService and the semantics of the key object for each of these calls.

## System.Data.Entity.IDatabaseInitializer<TContext\>  

**Version introduced**: EF6.0.0  

**Object returned**: A database initializer for the given context type  

**Key**: Not used; will be null  

## Func<System.Data.Entity.Migrations.Sql.MigrationSqlGenerator\>  

**Version introduced**: EF6.0.0

**Object returned**: A factory to create a SQL generator that can be used for Migrations and other actions that cause a database to be created, such as database creation with database initializers.  

**Key**: A string containing the ADO.NET provider invariant name specifying the type of database for which SQL will be generated. For example, the SQL Server SQL generator is returned for the key "System.Data.SqlClient".  

>[!NOTE]
> For more details on provider-related services in EF6 see the [EF6 provider model](xref:ef6/fundamentals/providers/provider-model) section.  

## System.Data.Entity.Core.Common.DbProviderServices  

**Version introduced**: EF6.0.0  

**Object returned**: The EF provider to use for a given provider invariant name  

**Key**: A string containing the ADO.NET provider invariant name specifying the type of database for which a provider is needed. For example, the SQL Server provider is returned for the key "System.Data.SqlClient".  

>[!NOTE]
> For more details on provider-related services in EF6 see the [EF6 provider model](xref:ef6/fundamentals/providers/provider-model) section.  

## System.Data.Entity.Infrastructure.IDbConnectionFactory  

**Version introduced**: EF6.0.0  

**Object returned**: The connection factory that will be used when EF creates a database connection by convention. That is, when no connection or connection string is given to EF, and no connection string can be found in the `app.config` or `web.config`, then this service is used to create a connection by convention. Changing the connection factory can allow EF to use a different type of database (for example, SQL Server Compact Edition) by default. Never store passwords or other sensitive data in configuration provider code or in plain text configuration files. Specify secrets outside of the project so that they can't be accidentally committed to a source code repository. Consider protecting the contents of the configuration file using [Protected Configuration](/dotnet/framework/data/adonet/connection-strings-and-configuration-files#encrypt-configuration-file-sections-using-protected-configuration).

**Key**: Not used; will be null  

>[!NOTE]
> For more details on provider-related services in EF6 see the [EF6 provider model](xref:ef6/fundamentals/providers/provider-model) section.  

## System.Data.Entity.Infrastructure.IManifestTokenService  

**Version introduced**: EF6.0.0  

**Object returned**: A service that can generate a provider manifest token from a connection. This service is typically used in two ways. First, it can be used to avoid Code First connecting to the database when building a model. Second, it can be used to force Code First to build a model for a specific database version -- for example, to force a model for SQL Server 2005 even if sometimes SQL Server 2008 is used.  

**Object lifetime**: Singleton -- the same object may be used multiple times and concurrently by different threads  

**Key**: Not used; will be null  

## System.Data.Entity.Infrastructure.IDbProviderFactoryService  

**Version introduced**: EF6.0.0  

**Object returned**: A service that can obtain a provider factory from a given connection. On .NET 4.5 the provider is publicly accessible from the connection. On .NET 4 the default implementation of this service uses some heuristics to find the matching provider. If these fail then a new implementation of this service can be registered to provide an appropriate resolution.  

**Key**: Not used; will be null  

## Func<DbContext, System.Data.Entity.Infrastructure.IDbModelCacheKey\>  

**Version introduced**: EF6.0.0  

**Object returned**: A factory that will generate a model cache key for a given context. By default, EF caches one model per DbContext type per provider. A different implementation of this service can be used to add other information, such as schema name, to the cache key.  

**Key**: Not used; will be null  

## System.Data.Entity.Spatial.DbSpatialServices  

**Version introduced**: EF6.0.0  

**Object returned**: An EF spatial provider that adds support to the basic EF provider for geography and geometry spatial types.  

**Key**: DbSpatialServices is asked for in two ways. First, provider-specific spatial services are requested using a DbProviderInfo object (which contains invariant name and manifest token) as the key. Second, DbSpatialServices can be asked for with no key. This is used to resolve the "global spatial provider" which is used when creating stand-alone DbGeography or DbGeometry types.  

>[!NOTE]
> For more details on provider-related services in EF6 see the [EF6 provider model](xref:ef6/fundamentals/providers/provider-model) section.  

## Func<System.Data.Entity.Infrastructure.IDbExecutionStrategy\>  

**Version introduced**: EF6.0.0  

**Object returned**: A factory to create a service that allows a provider to implement retries or other behavior when queries and commands are executed against the database. If no implementation is provided, then EF will simply execute the commands and propagate any exceptions thrown. For SQL Server this service is used to provide a retry policy which is especially useful when running against cloud-based database servers such as SQL Azure.  

**Key**: An ExecutionStrategyKey object that contains the provider invariant name and optionally a server name for which the execution strategy will be used.  

>[!NOTE]
> For more details on provider-related services in EF6 see the [EF6 provider model](xref:ef6/fundamentals/providers/provider-model) section.  

## Func<DbConnection, string, System.Data.Entity.Migrations.History.HistoryContext\>  

**Version introduced**: EF6.0.0  

**Object returned**: A factory that allows a provider to configure the mapping of the HistoryContext to the `__MigrationHistory` table used by EF Migrations. The HistoryContext is a Code First DbContext and can be configured using the normal fluent API to change things like the name of the table and the column mapping specifications.  

**Key**: Not used; will be null  

>[!NOTE]
> For more details on provider-related services in EF6 see the [EF6 provider model](xref:ef6/fundamentals/providers/provider-model) section.  

## System.Data.Common.DbProviderFactory  

**Version introduced**: EF6.0.0  

**Object returned**: The ADO.NET provider to use for a given provider invariant name.  

**Key**: A string containing the ADO.NET provider invariant name  

>[!NOTE]
> This service is not usually changed directly since the default implementation uses the normal ADO.NET provider registration. For more details on provider-related services in EF6 see the [EF6 provider model](xref:ef6/fundamentals/providers/provider-model) section.  

## System.Data.Entity.Infrastructure.IProviderInvariantName  

**Version introduced**: EF6.0.0  

**Object returned**: a service that is used to determine a provider invariant name for a given type of DbProviderFactory. The default implementation of this service uses the ADO.NET provider registration. This means that if the ADO.NET provider is not registered in the normal way because DbProviderFactory is being resolved by EF, then it will also be necessary to resolve this service.  

**Key**: The DbProviderFactory instance for which an invariant name is required.  

>[!NOTE]
> For more details on provider-related services in EF6 see the [EF6 provider model](xref:ef6/fundamentals/providers/provider-model) section.  

## System.Data.Entity.Core.Mapping.ViewGeneration.IViewAssemblyCache  

**Version introduced**: EF6.0.0  

**Object returned**: a cache of the assemblies that contain pre-generated views. A replacement is typically used to let EF know which assemblies contain pre-generated views without doing any discovery.  

**Key**: Not used; will be null  

## System.Data.Entity.Infrastructure.Pluralization.IPluralizationService

**Version introduced**: EF6.0.0  

**Object returned**: a service used by EF to pluralize and singularize names. By default an English pluralization service is used.  

**Key**: Not used; will be null  

## System.Data.Entity.Infrastructure.Interception.IDbInterceptor  

**Version introduced**: EF6.0.0

**Objects returned**: Any interceptors that should be registered when the application starts. Note that these objects are requested using the GetServices call and all interceptors returned by any dependency resolver will registered.

**Key**: Not used; will be null.  

## Func<System.Data.Entity.DbContext, Action<string\>, System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter\>  

**Version introduced**: EF6.0.0  

**Object returned**: A factory that will be used to create the database log formatter that will be used when the context.Database.Log property is set on the given context.  

**Key**: Not used; will be null.  

## Func<System.Data.Entity.DbContext\>  

**Version introduced**: EF6.1.0  

**Object returned**: A factory that will be used to create context instances for Migrations when the context does not have an accessible parameterless constructor.  

**Key**: The Type object for the type of the derived DbContext for which a factory is needed.  

## Func<System.Data.Entity.Core.Metadata.Edm.IMetadataAnnotationSerializer\>  

**Version introduced**: EF6.1.0  

**Object returned**: A factory that will be used to create serializers for serialization of strongly-typed custom annotations such that they can be serialized and deserialized into XML for use in Code First Migrations.  

**Key**: The name of the annotation that is being serialized or deserialized.  

## Func<System.Data.Entity.Infrastructure.TransactionHandler\>  

**Version introduced**: EF6.1.0  

**Object returned**: A factory that will be used to create handlers for transactions so that special handling can be applied for situations such as handling commit failures.  

**Key**: An ExecutionStrategyKey object that contains the provider invariant name and optionally a server name for which the transaction handler will be used.  
