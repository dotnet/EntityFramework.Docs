---
title: "Using migrate.exe - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 989ea862-e936-4c85-926a-8cfbef5df5b8
caps.latest.revision: 3
---
# Using migrate.exe
Code First Migrations can be used to update a database from inside visual studio, but can also be executed via the command line tool migrate.exe. This page will give a quick overview on how to use migrate.exe to execute migrations against a database.

> [!NOTE]
> This article assumes you know how to use Code First Migrations in basic scenarios. If you don’t, then you’ll need to read [Code First Migrations](~/ef6/modeling/code-first/migrations/index.md) before continuing.

## Copy migrate.exe

When you install Entity Framework using NuGet migrate.exe will be inside the tools folder of the downloaded package. In &lt;project folder&gt;\\packages\\EntityFramework.&lt;version&gt;\\tools

Once you have migrate.exe then you need to copy it to the location of the assembly that contains your migrations.

If your application targets .NET 4, and not 4.5, then you will need to copy the **Redirect.config** into the location as well and rename it **migrate.exe.config**. This is so that migrate.exe gets the correct binding redirects to be able to locate the Entity Framework assembly.

| .NET 4.5                                   | .NET 4.0                                   |
|:-------------------------------------------|:-------------------------------------------|
| ![Net45Files](~/ef6/media/net45files.png)  | ![Net40Files](~/ef6/media/net40files.png)  |

> [!NOTE]
> migrate.exe doesn't support x64 assemblies.

## Using Migrate.exe

Once you have moved migrate.exe to the correct folder then you should be able to use it to execute migrations against the database. All the utility is designed to do is execute migrations. It cannot generate migrations or create a SQL script.

### See options

``` console
Migrate.exe /?
```

The above will display the help page associated with this utility, note that you will need to have the EntityFramework.dll in the same location that you are running migrate.exe in order for this to work.

### Migrate to the latest migration

``` console
Migrate.exe MyMvcApplication.dll /startupConfigurationFile=”..\\web.config”
```

When running migrate.exe the only mandatory parameter is the assembly, which is the assembly that contains the migrations that you are trying to run, but it will use all convention based settings if you do not specify the configuration file.

### Migrate to a specific migration

``` console
Migrate.exe MyApp.exe /startupConfigurationFile=”MyApp.exe.config” /targetMigration=”AddTitle”
```

If you want to run migrations up to a specific migration, then you can specify the name of the migration. This will run all previous migrations as required until getting to the migration specified.

### Specify working directory

``` console
Migrate.exe MyApp.exe /startupConfigurationFile=”MyApp.exe.config” /startupDirectory=”c:\\MyApp”
```

If you assembly has dependencies or reads files relative to the working directory then you will need to set startupDirectory.

### Specify migration configuration to use

``` console
Migrate.exe MyAssembly CustomConfig /startupConfigurationFile=”..\\web.config”
```

If you have multiple migration configuration classes, classes inheriting from DbMigrationConfiguration, then you need to specify which is to be used for this execution. This is specified by providing the optional second parameter without a switch as above.

### Provide connection string

``` console
Migrate.exe BlogDemo.dll /connectionString=”Data Source=localhost;Initial Catalog=BlogDemo;Integrated Security=SSPI” /connectionProviderName=”System.Data.SqlClient”
```

If you wish to specify a connection string at the command line then you must also provide the provider name. Not specifying the provider name will cause an exception.

## Common Problems

| Error Message                                                                                                                                                                                                                                                                                                                      | Solution                                                                                                                                                                                                                                                                                             |
|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Unhandled Exception: System.IO.FileLoadException:  Could not load file or assembly 'EntityFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)         | This typically means that you are running a .NET 4 application without the Redirect.config file. You need to copy the Redirect.config to the same location as migrate.exe and rename it to migrate.exe.config.                                                                                       |
| Unhandled Exception: System.IO.FileLoadException: Could not load file or assembly 'EntityFramework, Version=4.4.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)          | This exception means that you are running a .NET 4.5 application with the Redirect.config copied to the migrate.exe location. If your app is .NET 4.5 then you do not need to have the config file with the redirects inside. Delete the migrate.exe.config file.                                    |
| ERROR: Unable to update database to match the current model because there are pending changes and automatic migration is disabled. Either write the pending model changes to a code-based migration or enable automatic migration. Set DbMigrationsConfiguration.AutomaticMigrationsEnabled to true to enable automatic migration. | This error occurs if running migrate when you haven’t created a migration to cope with changes made to the model, and the database does not match the model. Adding a property to a model class then running migrate.exe without creating a migration to upgrade the database is an example of this. |
| ERROR: Type is not resolved for member 'System.Data.Entity.Migrations.Design.ToolingFacade+UpdateRunner,EntityFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'.                                                                                                                                       | This error can be caused by specifying an incorrect startup directory. This must be the location of migrate.exe                                                                                                                                                                                      |
| Unhandled Exception: System.NullReferenceException: Object reference not set to an instance of an object. <br/>   at System.Data.Entity.Migrations.Console.Program.Main(String[] args)                                                                                                                                             | This can be caused by not specifying a required parameter for a scenario that you are using. For example specifying a connection string without specifying the provider name.                                                                                                                        |
| ERROR: More than one migrations configuration type was found in the assembly 'ClassLibrary1'. Specify the name of the one to use.                                                                                                                                                                                                  | As the error states, there is more than one configuration class in the given assembly. You must use the /configurationType switch to specify which to use.                                                                                                                                           |
| ERROR: Could not load file or assembly ‘&lt;assemblyName&gt;’ or one of its dependencies. The given assembly name or codebase was invalid. (Exception from HRESULT: 0x80131047)                                                                                                                                                    | This can be caused by specifying an assembly name incorrectly or not having                                                                                                                                                                                                                          |
| ERROR: Could not load file or assembly ‘&lt;assemblyName&gt;' or one of its dependencies. An attempt was made to load a program with an incorrect format.                                                                                                                                                                          | This happens if you are trying to run migrate.exe against an x64 application. EF 5.0 and below will only work on x86.                                                                                                                                                                                |
