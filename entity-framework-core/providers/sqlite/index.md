---
uid: providers/sqlite/index
---
# SQLite

> [!WARNING]
> This documentation is for EF Core. For EF6.x and earlier release see [http://msdn.com/data/ef](http://msdn.com/data/ef).

This database provider allows Entity Framework Core to be used with SQLite. The provider is maintained as part of the [EntityFramework GitHub project](https://github.com/aspnet/EntityFramework).

## Install

Install the [Microsoft.EntityFrameworkCore.SQLite NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SQLite/).

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text

   PM>  Install-Package Microsoft.EntityFrameworkCore.SQLite
````

## Get Started

The following resources will help you get started with this provider.
* [Local SQLite on UWP](../../platforms/uwp/getting-started.md)

* [.NET Core Application to New SQLite Database](../../platforms/netcore/new-db-sqlite.md)

* [Unicorn Clicker Sample Application](https://github.com/rowanmiller/UnicornStore/tree/master/UnicornClicker/UWP)

* [Unicorn Packer Sample Application](https://github.com/rowanmiller/UnicornStore/tree/master/UnicornPacker)

## Supported Database Engines

* SQLite (3.7 onwards)

## Supported Platforms

* Full .NET (4.5.1 onwards)

* .NET Core

* Mono (4.2.0 onwards)

* Universal Windows Platform

## Limitations

See [SQLite Limitations](limitations.md) for some important limitations of the SQLite provider.
