---
title: Configuration File Settings - EF6
description: Configuration file settings in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/configuring/config-file
---
# Configuration File Settings
Entity Framework allows a number of settings to be specified from the configuration file. In general EF follows a ‘convention over configuration’ principle: all the settings discussed in this post have a default behavior, you only need to worry about changing the setting when the default no longer satisfies your requirements. 

## Configuration data guidelines

* Never store passwords or other sensitive data in configuration provider code or in plain text configuration files.
* Don't use production secrets in development or test environments.
* Specify secrets outside of the project so that they can't be accidentally committed to a source code repository.
* Consider protecting the contents of the configuration file using [Protected Configuration](/dotnet/framework/data/adonet/connection-strings-and-configuration-files#encrypt-configuration-file-sections-using-protected-configuration).

[!INCLUDE [managed-identities-test-non-production](~/core/includes/managed-identities-test-non-production.md)]

## A Code-Based Alternative  

All of these settings can also be applied using code. Starting in EF6 we introduced [code-based configuration](xref:ef6/fundamentals/configuring/code-based), which provides a central way of applying configuration from code. Prior to EF6, configuration can still be applied from code but you need to use various APIs to configure different areas. The configuration file option allows these settings to be easily changed during deployment without updating your code.

## The Entity Framework Configuration Section  

Starting with EF4.1 you could set the database initializer for a context using the **appSettings** section of the configuration file. In EF 4.3 we introduced the custom **entityFramework** section to handle the new settings. Entity Framework will still recognize database initializers set using the old format, but we recommend moving to the new format where possible.

The **entityFramework** section was automatically added to the configuration file of your project when you installed the EntityFramework NuGet package.  

``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
    <section name="entityFramework"
       type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=4.3.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
  </configSections>
</configuration>
```  

## Connection Strings  

[This page](xref:ef6/fundamentals/configuring/connection-strings) provides more details on how Entity Framework determines the database to be used, including connection strings in the configuration file.  

Connection strings go in the standard **connectionStrings** element and do not require the **entityFramework** section.  

Code First based models use normal ADO.NET connection strings. For example:  

``` xml
<connectionStrings>
  <add name="BlogContext"  
        providerName="System.Data.SqlClient"  
        connectionString="Server=.\SQLEXPRESS;Database=Blogging;Integrated Security=True;"/>
</connectionStrings>
```  

EF Designer based models use special EF connection strings. For example:  

``` xml  
<connectionStrings>
  <add name="BlogContext"  
    connectionString=
      "metadata=
        res://*/BloggingModel.csdl|
        res://*/BloggingModel.ssdl|
        res://*/BloggingModel.msl;
      provider=System.Data.SqlClient;
      provider connection string=
        &quot;data source=(localdb)\mssqllocaldb;
        initial catalog=Blogging;
        integrated security=True;
        multipleactiveresultsets=True;&quot;"
     providerName="System.Data.EntityClient" />
</connectionStrings>
```

## Code-Based Configuration Type (EF6 Onwards)  

Starting with EF6, you can specify the DbConfiguration for EF to use for [code-based configuration](xref:ef6/fundamentals/configuring/code-based) in your application. In most cases you don't need to specify this setting as EF will automatically discover your DbConfiguration. For details of when you may need to specify DbConfiguration in your config file see the **Moving DbConfiguration** section of [Code-Based Configuration](xref:ef6/fundamentals/configuring/code-based).  

To set a DbConfiguration type, you specify the assembly qualified type name in the **codeConfigurationType** element.  

> [!NOTE]
> An assembly qualified name is the namespace qualified name, followed by a comma, then the assembly that the type resides in. You can optionally also specify the assembly version, culture and public key token.  

``` xml
<entityFramework codeConfigurationType="MyNamespace.MyConfiguration, MyAssembly">
</entityFramework>
```  

## EF Database Providers (EF6 Onwards)  

Prior to EF6, Entity Framework-specific parts of a database provider had to be included as part of the core ADO.NET provider. Starting with EF6, the EF specific parts are now managed and registered separately.  

Normally you won't need to register providers yourself. This will typically be done by the provider when you install it.  

Providers are registered by including a **provider** element under the **providers** child section of the **entityFramework** section. There are two required attributes for a provider entry:  

- **invariantName** identifies the core ADO.NET provider that this EF provider targets  
- **type** is the assembly qualified type name of the EF provider implementation  

> [!NOTE]
> An assembly qualified name is the namespace qualified name, followed by a comma, then the assembly that the type resides in. You can optionally also specify the assembly version, culture and public key token.  

As an example here is the entry created to register the default SQL Server provider when you install Entity Framework.  

``` xml  
<providers>
  <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
</providers>
```  

## Interceptors (EF6.1 Onwards)  

Starting with EF6.1 you can register interceptors in the configuration file. Interceptors allow you to run additional logic when EF performs certain operations, such as executing database queries, opening connections, etc.  

Interceptors are registered by including an **interceptor** element under the **interceptors** child section of the **entityFramework** section. For example, the following configuration registers the built-in **DatabaseLogger** interceptor that will log all database operations to the Console.  

``` xml  
<interceptors>
  <interceptor type="System.Data.Entity.Infrastructure.Interception.DatabaseLogger, EntityFramework"/>
</interceptors>
```  

### Logging Database Operations to a File (EF6.1 Onwards)  

Registering interceptors via the config file is especially useful when you want to add logging to an existing application to help debug an issue. **DatabaseLogger** supports logging to a file by supplying the file name as a constructor parameter.  

``` xml  
<interceptors>
  <interceptor type="System.Data.Entity.Infrastructure.Interception.DatabaseLogger, EntityFramework">
    <parameters>
      <parameter value="C:\Temp\LogOutput.txt"/>
    </parameters>
  </interceptor>
</interceptors>
```  

By default this will cause the log file to be overwritten with a new file each time the app starts. To instead append to the log file if it already exists use something like:  

``` xml  
<interceptors>
  <interceptor type="System.Data.Entity.Infrastructure.Interception.DatabaseLogger, EntityFramework">
    <parameters>
      <parameter value="C:\Temp\LogOutput.txt"/>
      <parameter value="true" type="System.Boolean"/>
    </parameters>
  </interceptor>
</interceptors>
```  

For additional information on **DatabaseLogger** and registering interceptors, see the blog post [EF 6.1: Turning on logging without recompiling](https://blog.oneunicorn.com/2014/02/09/ef-6-1-turning-on-logging-without-recompiling/).  

## Code First Default Connection Factory  

The configuration section allows you to specify a default connection factory that Code First should use to locate a database to use for a context. The default connection factory is only used when no connection string has been added to the configuration file for a context.  

When you installed the EF NuGet package a default connection factory was registered that points to either SQL Express or LocalDB, depending on which one you have installed.  

To set a connection factory, you specify the assembly qualified type name in the **defaultConnectionFactory** element.  

> [!NOTE]
> An assembly qualified name is the namespace qualified name, followed by a comma, then the assembly that the type resides in. You can optionally also specify the assembly version, culture and public key token.  

Here is an example of setting your own default connection factory:  

``` xml  
<entityFramework>
  <defaultConnectionFactory type="MyNamespace.MyCustomFactory, MyAssembly"/>
</entityFramework>
```  

The above example requires the custom factory to have a parameterless constructor. If needed, you can specify constructor parameters using the **parameters** element.  

For example, the SqlCeConnectionFactory, that is included in Entity Framework, requires you to supply a provider invariant name to the constructor. The provider invariant name identifies the version of SQL Compact you want to use. The following configuration will cause contexts to use SQL Compact version 4.0 by default.  

``` xml  
<entityFramework>
  <defaultConnectionFactory type="System.Data.Entity.Infrastructure.SqlCeConnectionFactory, EntityFramework">
    <parameters>
      <parameter value="System.Data.SqlServerCe.4.0" />
    </parameters>
  </defaultConnectionFactory>
</entityFramework>
```  

If you don’t set a default connection factory, Code First uses the SqlConnectionFactory, pointing to `.\SQLEXPRESS`. SqlConnectionFactory also has a constructor that allows you to override parts of the connection string. If you want to use a SQL Server instance other than `.\SQLEXPRESS` you can use this constructor to set the server.  

The following configuration will cause Code First to use **MyDatabaseServer** for contexts that don’t have an explicit connection string set.  

``` xml  
<entityFramework>
  <defaultConnectionFactory type="System.Data.Entity.Infrastructure.SqlConnectionFactory, EntityFramework">
    <parameters>
      <parameter value="Data Source=MyDatabaseServer; Integrated Security=True; MultipleActiveResultSets=True" />
    </parameters>
  </defaultConnectionFactory>
</entityFramework>
```  

By default, it’s assumed that constructor arguments are of type string. You can use the type attribute to change this.  

``` xml
<parameter value="2" type="System.Int32" />
```  

## Database Initializers  

Database initializers are configured on a per-context basis. They can be set in the configuration file using the **context** element. This element uses the assembly qualified name to identify the context being configured.  

By default, Code First contexts are configured to use the CreateDatabaseIfNotExists initializer. There is a **disableDatabaseInitialization** attribute on the **context** element that can be used to disable database initialization.  

For example, the following configuration disables database initialization for the Blogging.BlogContext context defined in MyAssembly.dll.  

``` xml  
<contexts>
  <context type=" Blogging.BlogContext, MyAssembly" disableDatabaseInitialization="true" />
</contexts>
```  

You can use the **databaseInitializer** element to set a custom initializer.  

``` xml
<contexts>
  <context type=" Blogging.BlogContext, MyAssembly">
    <databaseInitializer type="Blogging.MyCustomBlogInitializer, MyAssembly" />
  </context>
</contexts>
```  

Constructor parameters use the same syntax as default connection factories.  

``` xml  
<contexts>
  <context type=" Blogging.BlogContext, MyAssembly">
    <databaseInitializer type="Blogging.MyCustomBlogInitializer, MyAssembly">
      <parameters>
        <parameter value="MyConstructorParameter" />
      </parameters>
    </databaseInitializer>
  </context>
</contexts>
```  

You can configure one of the generic database initializers that are included in Entity Framework. The **type** attribute uses the .NET Framework format for generic types.  

For example, if you are using Code First Migrations, you can configure the database to be migrated automatically using the `MigrateDatabaseToLatestVersion<TContext, TMigrationsConfiguration>` initializer.  

``` xml
<contexts>
  <context type="Blogging.BlogContext, MyAssembly">
    <databaseInitializer type="System.Data.Entity.MigrateDatabaseToLatestVersion`2[[Blogging.BlogContext, MyAssembly], [Blogging.Migrations.Configuration, MyAssembly]], EntityFramework" />
  </context>
</contexts>
```
