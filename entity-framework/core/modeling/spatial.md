---
title: Spatial Data - EF Core
description: Using spatial data in an Entity Framework Core model
author: SamMonoRT
ms.date: 11/15/2021
uid: core/modeling/spatial
---
# Spatial Data

Spatial data represents the physical location and the shape of objects. Many databases provide support for this type of data so it can be indexed and queried alongside other data. Common scenarios include querying for objects within a given distance from a location, or selecting the object whose border contains a given location. EF Core supports mapping to spatial data types using the NetTopologySuite spatial library.

## Installing

In order to use spatial data with EF Core, you need to install the appropriate supporting NuGet package. Which package you need to install depends on the provider you're using.

EF Core Provider                        | Spatial NuGet Package
--------------------------------------- | ---------------------
Microsoft.EntityFrameworkCore.SqlServer | [Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite)
Microsoft.EntityFrameworkCore.Sqlite    | [Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite)
Microsoft.EntityFrameworkCore.InMemory  | [NetTopologySuite](https://www.nuget.org/packages/NetTopologySuite)
Npgsql.EntityFrameworkCore.PostgreSQL   | [Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite)
Pomelo.EntityFrameworkCore.MySql        | [Pomelo.EntityFrameworkCore.MySql.NetTopologySuite](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.NetTopologySuite)
Devart.Data.MySql.EFCore                | [Devart.Data.MySql.EFCore.NetTopologySuite](https://www.nuget.org/packages/Devart.Data.MySql.EFCore.NetTopologySuite)
Devart.Data.Oracle.EFCore               | [Devart.Data.Oracle.EFCore.NetTopologySuite](https://www.nuget.org/packages/Devart.Data.Oracle.EFCore.NetTopologySuite)
Devart.Data.PostgreSql.EFCore           | [Devart.Data.PostgreSql.EFCore.NetTopologySuite](https://www.nuget.org/packages/Devart.Data.PostgreSql.EFCore.NetTopologySuite)
Devart.Data.SQLite.EFCore               | [Devart.Data.SQLite.EFCore.NetTopologySuite](https://www.nuget.org/packages/Devart.Data.SQLite.EFCore.NetTopologySuite)
Teradata.EntityFrameworkCore            | [Teradata.EntityFrameworkCore.NetTopologySuite](https://www.nuget.org/packages/Teradata.EntityFrameworkCore.NetTopologySuite)

## NetTopologySuite

[NetTopologySuite](https://nettopologysuite.github.io/NetTopologySuite/) (NTS) is a spatial library for .NET. EF Core enables mapping to spatial data types in the database by using NTS types in your model.

To enable mapping to spatial types via NTS, call the UseNetTopologySuite method on the provider's DbContext options builder. For example, with SQL Server you'd call it like this.

[!code-csharp[](../../../samples/core/Spatial/SqlServer/Models/WideWorldImportersContext.cs?name=snippet_UseNetTopologySuite)]

There are several spatial data types. Which type you use depends on the types of shapes you want to allow. Here is the hierarchy of NTS types that you can use for properties in your model. They're located within the `NetTopologySuite.Geometries` namespace.

* Geometry
  * Point
  * LineString
  * Polygon
  * GeometryCollection
    * MultiPoint
    * MultiLineString
    * MultiPolygon

> [!WARNING]
> CircularString, CompoundCurve, and CurePolygon aren't supported by NTS.

Using the base Geometry type allows any type of shape to be specified by the property.

## Longitude and Latitude

Coordinates in NTS are in terms of X and Y values. To represent longitude and latitude, use X for longitude and Y for latitude. Note that this is **backwards** from the `latitude, longitude` format in which you typically see these values.

## Querying Data

The following entity classes could be used to map to tables in the [Wide World Importers sample database](https://go.microsoft.com/fwlink/?LinkID=800630).

[!code-csharp[](../../../samples/core/Spatial/SqlServer/Models/City.cs?name=snippet_City)]

[!code-csharp[](../../../samples/core/Spatial/SqlServer/Models/Country.cs?name=snippet_Country)]

In LINQ, the NTS methods and properties available as database functions will be translated to SQL. For example, the Distance and Contains methods are translated in the following queries. See your provider's documentation for which methods are supported.

[!code-csharp[](../../../samples/core/Spatial/SqlServer/Program.cs?name=snippet_Distance)]

[!code-csharp[](../../../samples/core/Spatial/SqlServer/Program.cs?name=snippet_Contains)]

## Reverse engineering

The spatial NuGet packages also enable [reverse engineering](xref:core/managing-schemas/scaffolding) models with spatial properties, but you need to install the package ***before*** running `Scaffold-DbContext` or `dotnet ef dbcontext scaffold`. If you don't, you'll receive warnings about not finding type mappings for the columns and the columns will be skipped.

## SRID Ignored during client operations

NTS ignores SRID values during operations. It assumes a planar coordinate system. This means that if you specify coordinates in terms of longitude and latitude, some client-evaluated values like distance, length, and area will be in degrees, not meters. For more meaningful values, you first need to project the coordinates to another coordinate system using a library like [ProjNet (for GeoAPI)](https://github.com/NetTopologySuite/ProjNet4GeoAPI).

> [!NOTE]
> Use the newer [ProjNet NuGet package](https://www.nuget.org/packages/ProjNet/), **not** the older package called ProjNet4GeoAPI.

If an operation is server-evaluated by EF Core via SQL, the result's unit will be determined by the database.

Here is an example of using ProjNet to calculate the distance between two cities.

[!code-csharp[](../../../samples/core/Spatial/Projections/GeometryExtensions.cs?name=snippet_GeometryExtensions)]

[!code-csharp[](../../../samples/core/Spatial/Projections/Program.cs?name=snippet_ProjectTo)]

> [!NOTE]
> 4326 refers to WGS 84, a standard used in GPS and other geographic systems.

## Additional resources

### Database-specific information

Be sure to read your provider's documentation for additional information on working with spatial data.

* [Spatial Data in the SQL Server Provider](xref:core/providers/sql-server/spatial)
* [Spatial Data in the SQLite Provider](xref:core/providers/sqlite/spatial)
* [Spatial Data in the Npgsql Provider](https://www.npgsql.org/efcore/mapping/nts.html)

### Other resources

* [NetTopologySuite Docs](https://nettopologysuite.github.io/NetTopologySuite/)
* [.NET Data Community Standup session](https://www.youtube.com/watch?v=IHslY5rrxD0&list=PLdo4fOcmZ0oX-DBuRG4u58ZTAJgBAeQ-t&index=15), focusing on spatial data and NetTopologySuite.
