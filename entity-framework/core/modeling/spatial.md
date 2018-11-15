---
title: Spatial Data - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/01/2018
ms.assetid: 2BDE29FC-4161-41A0-841E-69F51CCD9341
uid: core/modeling/spatial
---
# Spatial Data

> [!NOTE]
> This feature is new in EF Core 2.2.

Spatial data represents the physical location and the shape of objects. Many databases provide support for this type of data so it can be indexed and queried alongside other data. Common scenarios include querying for objects within a given distance from a location, or selecting the object whose border contains a given location. EF Core supports mapping to spatial data types using the [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) spatial library.

## Installing

In order to use spatial data with EF Core, you need to install the appropriate supporting NuGet package. Which package you need to install depends on the provider you're using.

EF Core Provider                        | Spatial NuGet Package
--------------------------------------- | ---------------------
Microsoft.EntityFrameworkCore.SqlServer | [Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite)
Microsoft.EntityFrameworkCore.Sqlite    | [Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite)
Microsoft.EntityFrameworkCore.InMemory  | [NetTopologySuite](https://www.nuget.org/packages/NetTopologySuite)
Npgsql.EntityFrameworkCore.PostgreSQL   | [Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite)

## Reverse engineering

The spatial NuGet packages also enable [reverse engineering](../managing-schemas/scaffolding.md) models with spatial properties, but you need to install the package ***before*** running `Scaffold-DbContext` or `dotnet ef dbcontext scaffold`. If you don't, you'll receive warnings about not finding type mappings for the columns and the columns will be skipped.

## NetTopologySuite (NTS)

NetTopologySuite is a spatial library for .NET. EF Core enables mapping to spatial data types in the database by using NTS types in your model.

To enable mapping to spatial types via NTS, call the UseNetTopologySuite method on the provider's DbContext options builder. For example, with SQL Server you'd call it like this.

``` csharp
optionsBuilder.UseSqlServer(
    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WideWorldImporters",
    x => x.UseNetTopologySuite());
```

There are several spatial data types. Which type you use depends on the types of shapes you want to allow. Here is the hierarchy of NTS types that you can use for properties in your model. They're located within the `NetTopologySuite.Geometries` namespace. Corresponding interfaces in the GeoAPI package (`GeoAPI.Geometries` namespace) can also be used.

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

The following entity classes could be used to map to tables in the [Wide World Importers sample database](http://go.microsoft.com/fwlink/?LinkID=800630).

``` csharp
[Table("Cities", Schema = "Application"))]
class City
{
    public int CityID { get; set; }

    public string CityName { get; set; }

    public IPoint Location { get; set; }
}

[Table("Countries", Schema = "Application"))]
class Country
{
    public int CountryID { get; set; }

    public string CountryName { get; set; }

    // Database includes both Polygon and MultiPolygon values
    public IGeometry Border { get; set; }
}
```

### Creating values

You can use constructors to create geometry objects; however, NTS recommends using a geometry factory instead. This lets you specify a default SRID (the spatial reference system used by the coordinates) and gives you control over more advanced things like the precision model (used during calculations) and the coordinate sequence (determines which ordinates--dimensions and measures--are available).

``` csharp
var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
var currentLocation = geometryFactory.CreatePoint(-122.121512, 47.6739882);
```

> [!NOTE]
> 4326 refers to WGS 84, a standard used in GPS and other geographic systems.

### Longitude and Latitude

Coordinates in NTS are in terms of X and Y values. To represent longitude and latitude, use X for longitude and Y for latitude. Note that this is **backwards** from the `latitude, longitude` format in which you typically see these values.

### SRID Ignored during client operations

NTS ignores SRID values during operations. It assumes a planar coordinate system. This means that if you specify coordinates in terms of longitude and latitude, some client-evaluated values like distance, length, and area will be in degrees, not meters. For more meaningful values, you first need to project the coordinates to another coordinate system using a library like [ProjNet4GeoAPI](https://github.com/NetTopologySuite/ProjNet4GeoAPI) before calculating these values.

If an operation is server-evaluated by EF Core via SQL, the result's unit will be determined by the database.

Here is an example of using ProjNet4GeoAPI to calculate the distance between two cities.

``` csharp
static class GeometryExtensions
{
    static readonly IGeometryServices _geometryServices = NtsGeometryServices.Instance;
    static readonly ICoordinateSystemServices _coordinateSystemServices
        = new CoordinateSystemServices(
            new CoordinateSystemFactory(),
            new CoordinateTransformationFactory(),
            new Dictionary<int, string>
            {
                // Coordinate systems:

                // (3857 and 4326 included automatically)

                // This coordinate system covers the area of our data.
                // Different data requires a different coordinate system.
                [2855] =
                @"
                    PROJCS[""NAD83(HARN) / Washington North"",
                        GEOGCS[""NAD83(HARN)"",
                            DATUM[""NAD83_High_Accuracy_Regional_Network"",
                                SPHEROID[""GRS 1980"",6378137,298.257222101,
                                    AUTHORITY[""EPSG"",""7019""]],
                                AUTHORITY[""EPSG"",""6152""]],
                            PRIMEM[""Greenwich"",0,
                                AUTHORITY[""EPSG"",""8901""]],
                            UNIT[""degree"",0.01745329251994328,
                                AUTHORITY[""EPSG"",""9122""]],
                            AUTHORITY[""EPSG"",""4152""]],
                        PROJECTION[""Lambert_Conformal_Conic_2SP""],
                        PARAMETER[""standard_parallel_1"",48.73333333333333],
                        PARAMETER[""standard_parallel_2"",47.5],
                        PARAMETER[""latitude_of_origin"",47],
                        PARAMETER[""central_meridian"",-120.8333333333333],
                        PARAMETER[""false_easting"",500000],
                        PARAMETER[""false_northing"",0],
                        UNIT[""metre"",1,
                            AUTHORITY[""EPSG"",""9001""]],
                        AUTHORITY[""EPSG"",""2855""]]
                "
            });

    public static IGeometry ProjectTo(this IGeometry geometry, int srid)
    {
        var geometryFactory = _geometryServices.CreateGeometryFactory(srid);
        var transformation = _coordinateSystemServices.CreateTransformation(geometry.SRID, srid);

        return GeometryTransform.TransformGeometry(
            geometryFactory,
            geometry,
            transformation.MathTransform);
    }
}
```

``` csharp
var seattle = new Point(-122.333056, 47.609722) { SRID = 4326 };
var redmond = new Point(-122.123889, 47.669444) { SRID = 4326 };

var distance = seattle.ProjectTo(2855).Distance(redmond.ProjectTo(2855));
```

## Querying Data

In LINQ, the NTS methods and properties available as database functions will be translated to SQL. For example, the Distance and Contains methods are translated in the following queries. The table at the end of this article shows which members are supported by various EF Core providers.

``` csharp
var nearestCity = db.Cities
    .OrderBy(c => c.Location.Distance(currentLocation))
    .FirstOrDefault();

var currentCountry = db.Countries
    .FirstOrDefault(c => c.Border.Contains(currentLocation));
```

## SQL Server

If you're using SQL Server, there are some additional things you should be aware of.

### Geography or geometry

By default, spatial properties are mapped to `geography` columns in SQL Server. To use `geometry`, [configure the column type](xref:core/modeling/relational/data-types) in your model.

### Geography polygon rings

When using the `geography` column type, SQL Server imposes additional requirements on the exterior ring (or shell) and interior rings (or holes). The exterior ring must be oriented counterclockwise and the interior rings clockwise. NTS validates this before sending values to the database.

### FullGlobe

SQL Server has a non-standard geometry type to represent the full globe when using the `geography` column type. It also has a way to represent polygons based on the full globe (without an exterior ring). Neither of these are supported by NTS.

> [!WARNING]
> FullGlobe and polygons based on it aren't supported by NTS.

## SQLite

Here is some additional information for those using SQLite.

### Installing SpatiaLite

On Windows, the native mod_spatialite library is distributed as a NuGet package dependency. Other platforms need to install it separately. This is typically done using a software package manager. For example, you can use APT on Ubuntu and Homebrew on MacOS.

``` sh
# Ubuntu
apt-get install libsqlite3-mod-spatialite

# macOS
brew install libspatialite
```

### Configuring SRID

In SpatiaLite, columns need to specify an SRID per column. The default SRID is `0`. Specify a different SRID using the ForSqliteHasSrid method.

``` csharp
modelBuilder.Entity<City>().Property(c => c.Location)
    .ForSqliteHasSrid(4326);
```

### Dimension

Similar to SRID, a column's dimension (or ordinates) is also specified as part of the column. The default ordinates are X and Y. Enable additional ordinates (Z and M) using the ForSqliteHasDimension method.

``` csharp
modelBuilder.Entity<City>().Property(c => c.Location)
    .ForSqliteHasDimension(Ordinates.XYZ);
```

## Translated Operations

This table shows which NTS members are translated into SQL by each EF Core provider.

NetTopologySuite | SQL Server (geometry) | SQL Server (geography) | SQLite | Npgsql
--- |:---:|:---:|:---:|:---:
Geometry.Area | ✔ | ✔ | ✔ | ✔
Geometry.AsBinary() | ✔ | ✔ | ✔ | ✔
Geometry.AsText() | ✔ | ✔ | ✔ | ✔
Geometry.Boundary | ✔ | | ✔ | ✔
Geometry.Buffer(double) | ✔ | ✔ | ✔ | ✔
Geometry.Buffer(double, int) | | | ✔
Geometry.Centroid | ✔ | | ✔ | ✔
Geometry.Contains(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.ConvexHull() | ✔ | ✔ | ✔ | ✔
Geometry.CoveredBy(Geometry) | | | ✔ | ✔
Geometry.Covers(Geometry) | | | ✔ | ✔
Geometry.Crosses(Geometry) | ✔ | | ✔ | ✔
Geometry.Difference(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.Dimension | ✔ | ✔ | ✔ | ✔
Geometry.Disjoint(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.Distance(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.Envelope | ✔ | | ✔ | ✔
Geometry.EqualsExact(Geometry) | | | | ✔
Geometry.EqualsTopologically(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.GeometryType | ✔ | ✔ | ✔ | ✔
Geometry.GetGeometryN(int) | ✔ | | ✔ | ✔
Geometry.InteriorPoint | ✔ | | ✔
Geometry.Intersection(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.Intersects(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.IsEmpty | ✔ | ✔ | ✔ | ✔
Geometry.IsSimple | ✔ | | ✔ | ✔
Geometry.IsValid | ✔ | ✔ | ✔ | ✔
Geometry.IsWithinDistance(Geometry, double) | ✔ | | ✔
Geometry.Length | ✔ | ✔ | ✔ | ✔
Geometry.NumGeometries | ✔ | ✔ | ✔ | ✔
Geometry.NumPoints | ✔ | ✔ | ✔ | ✔
Geometry.OgcGeometryType | ✔ | ✔ | ✔
Geometry.Overlaps(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.PointOnSurface | ✔ | | ✔ | ✔
Geometry.Relate(Geometry, string) | ✔ | | ✔ | ✔
Geometry.Reverse() | | | ✔ | ✔
Geometry.SRID | ✔ | ✔ | ✔ | ✔
Geometry.SymmetricDifference(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.ToBinary() | ✔ | ✔ | ✔ | ✔
Geometry.ToText() | ✔ | ✔ | ✔ | ✔
Geometry.Touches(Geometry) | ✔ | | ✔ | ✔
Geometry.Union() | | | ✔
Geometry.Union(Geometry) | ✔ | ✔ | ✔ | ✔
Geometry.Within(Geometry) | ✔ | ✔ | ✔ | ✔
GeometryCollection.Count | ✔ | ✔ | ✔ | ✔
GeometryCollection[int] | ✔ | ✔ | ✔ | ✔
LineString.Count | ✔ | ✔ | ✔ | ✔
LineString.EndPoint | ✔ | ✔ | ✔ | ✔
LineString.GetPointN(int) | ✔ | ✔ | ✔ | ✔
LineString.IsClosed | ✔ | ✔ | ✔ | ✔
LineString.IsRing | ✔ | | ✔ | ✔
LineString.StartPoint | ✔ | ✔ | ✔ | ✔
MultiLineString.IsClosed | ✔ | ✔ | ✔ | ✔
Point.M | ✔ | ✔ | ✔ | ✔
Point.X | ✔ | ✔ | ✔ | ✔
Point.Y | ✔ | ✔ | ✔ | ✔
Point.Z | ✔ | ✔ | ✔ | ✔
Polygon.ExteriorRing | ✔ | ✔ | ✔ | ✔
Polygon.GetInteriorRingN(int) | ✔ | ✔ | ✔ | ✔
Polygon.NumInteriorRings | ✔ | ✔ | ✔ | ✔

## Additional resources

* [Spatial Data in SQL Server](https://docs.microsoft.com/sql/relational-databases/spatial/spatial-data-sql-server)
* [SpatiaLite Homepage](https://www.gaia-gis.it/fossil/libspatialite)
* [Npgsql Spatial Documentation](http://www.npgsql.org/efcore/mapping/nts.html)
* [PostGIS Documentation](http://postgis.net/documentation/)
