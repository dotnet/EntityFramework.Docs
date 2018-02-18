---
title: "LocalDb Bug in MVC4 Beta 1 Code First Applications - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 1052800e-8040-4d0d-a26b-07f48ddde6e3
caps.latest.revision: 3
---
# LocalDb Bug in MVC4 Beta 1 Code First Applications
> **Note**: The information on this page is out of date.  
The bug described below was fixed in the final release of MVC4.  

Historically Code First has used SQLEXPRESS as the default location to create databases. We needed to make some changes to this default because Visual Studio 11 now includes [LocalDb](https://msdn.microsoft.com/library/hh510202.aspx) rather than SQLEXPRESS.  

EF4.3.1 and later releases (including 5.0.0-beta1) take care of this by detecting what database locations are available when you install the EntityFramework NuGet package. A setting is then added to your config file to register either SQLEXPRESS or LocalDb as the default location to create Code First databases. If SQLEXPRESS is available it will be used, if not, LocalDb will be registered as the default. This ensures that Code First continues to work in both Visual Studio 2010 and Visual Studio 11.  

## The Issue in MVC4 Beta 1  

MVC4 Beta 1 still includes the EF4.1 package by default in new projects. In this older release there is no way of setting the default database location from the config file. If you are working in Visual Studio 11 then MVC4 will add a line of code to global.asax file to register LocalDb as the default. The issue is that this line of code is wrong and is missing an escape character in the server name:  

```  
Database.DefaultConnectionFactory = new SqlConnectionFactory(
  "Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");
```  

Notice that the server name is ‘(localdb)\\v11.0’. The problem is that ‘\\v’ is the escape sequence for ‘vertical tab’. The server name really should be ‘(localdb)\\\\v11.0’. **Be aware that simply adding in the missing slash is not the best way to fix this problem, see the ‘Fixing the Problem’ section for more details.**  

## Fixed in the Next Release of MVC4  

In the next release of MVC4 the EntityFramework package that is installed by default will be updated to the latest version and the offending line of code in global.asax will be removed and replaced with the correct entry in web.config.  

## Fixing the Problem in MVC4 Beta 1  

The best solution is to delete the offending line of code from global.asax and upgrade the EntityFramework package. We recommend upgrading to the EF 5.0.0-beta1 package.  

- Tools->Library Package Manager -> Package Manager Console
- Run the **Update-Package EntityFramework –IncludePrerelease** command

You can also install the latest non-prerelease package by excluding the ‘-IncludePrerelease’ flag from the above command.  

If you are unable to upgrade the EntityFramework NuGet package then you can add the missing slash into the server name in global.asax. If you chose this approach then be aware that any tooling, such as Code First Migrations, will not be aware that LocalDb is registered as the default because this code only runs when you MVC application is launched. Such tools will therefore try and connect to SQLEXPRESS, which remains as the default if no settings are provided. You should ensure you remove this line of code if you later upgrade to EF4.3.1 or a later version.  
