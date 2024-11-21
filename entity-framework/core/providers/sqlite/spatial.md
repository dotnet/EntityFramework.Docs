---
title: SQLite Database Provider - Spatial Data - EF Core
description: Using spatial data with the Entity Framework Core SQLite database provider
author: SamMonoRT
ms.date: 10/02/2020
uid: core/providers/sqlite/spatial
---
# Spatial Data in the SQLite EF Core Provider

This page includes additional information about using spatial data with the SQLite database provider. For general information about using spatial data in EF Core, see the main [Spatial Data](xref:core/modeling/spatial) documentation.

## Installing SpatiaLite

On Windows, the native `mod_spatialite` library is distributed as a [NuGet package](https://www.nuget.org/packages/mod_spatialite) dependency. Other platforms need to install it separately. This is typically done using a software package manager. For example, you can use APT on Debian and Ubuntu; and Homebrew on MacOS.

```bash
# Debian/Ubuntu
apt-get install libsqlite3-mod-spatialite

# macOS
brew install libspatialite
```

Unfortunately, newer versions of PROJ (a dependency of SpatiaLite) are incompatible with EF's default [SQLitePCLRaw bundle](/dotnet/standard/data/sqlite/custom-versions#bundles). You can work around this by using the system SQLite library instead.

```xml
<ItemGroup>
  <!-- Use bundle_sqlite3 instead with SpatiaLite on macOS and Linux -->
  <!--<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="3.1.0" />-->
  <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite.Core" Version="3.1.0" />
  <PackageReference Include="SQLitePCLRaw.bundle_sqlite3" Version="2.0.4" />

  <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite" Version="3.1.0" />
</ItemGroup>
```

On **macOS**, you'll also need set an environment variable before running your app so it uses Homebrew's version of SQLite. In Visual Studio for Mac, you can set this under **Project > Project Options > Run > Configurations > Default**

```bash
DYLD_LIBRARY_PATH=/usr/local/opt/sqlite/lib
```

## Configuring SRID

In SpatiaLite, columns need to specify an SRID per column. The default SRID is `0`. Specify a different SRID using the HasSrid method.

```csharp
modelBuilder.Entity<City>().Property(c => c.Location)
    .HasSrid(4326);
```

> [!NOTE]
> 4326 refers to WGS 84, a standard used in GPS and other geographic systems.

## Dimension

The default dimension (or ordinates) of a column is X and Y. To enable additional ordinates like Z or M, configure the column type.

```csharp
modelBuilder.Entity<City>().Property(c => c.Location)
    .HasColumnType("POINTZ");
```

## Spatial function mappings

This table shows which [NetTopologySuite](https://nettopologysuite.github.io/NetTopologySuite/) (NTS) members are translated into which SQL functions.

.NET                                        | SQL
------------------------------------------- | ---
geometry.Area                               | Area(@geometry)
geometry.AsBinary()                         | AsBinary(@geometry)
geometry.AsText()                           | AsText(@geometry)
geometry.Boundary                           | Boundary(@geometry)
geometry.Buffer(distance)                   | Buffer(@geometry, @distance)
geometry.Buffer(distance, quadrantSegments) | Buffer(@geometry, @distance, @quadrantSegments)
geometry.Centroid                           | Centroid(@geometry)
geometry.Contains(g)                        | Contains(@geometry, @g)
geometry.ConvexHull()                       | ConvexHull(@geometry)
geometry.CoveredBy(g)                       | CoveredBy(@geometry, @g)
geometry.Covers(g)                          | Covers(@geometry, @g)
geometry.Crosses(g)                         | Crosses(@geometry, @g)
geometry.Difference(other)                  | Difference(@geometry, @other)
geometry.Dimension                          | Dimension(@geometry)
geometry.Disjoint(g)                        | Disjoint(@geometry, @g)
geometry.Distance(g)                        | Distance(@geometry, @g)
geometry.Envelope                           | Envelope(@geometry)
geometry.EqualsTopologically(g)             | Equals(@geometry, @g)
geometry.GeometryType                       | GeometryType(@geometry)
geometry.GetGeometryN(n)                    | GeometryN(@geometry, @n + 1)
geometry.InteriorPoint                      | PointOnSurface(@geometry)
geometry.Intersection(other)                | Intersection(@geometry, @other)
geometry.Intersects(g)                      | Intersects(@geometry, @g)
geometry.IsEmpty                            | IsEmpty(@geometry)
geometry.IsSimple                           | IsSimple(@geometry)
geometry.IsValid                            | IsValid(@geometry)
geometry.IsWithinDistance(geom, distance)   | Distance(@geometry, @geom) <= @distance
geometry.Length                             | GLength(@geometry)
geometry.NumGeometries                      | NumGeometries(@geometry)
geometry.NumPoints                          | NumPoints(@geometry)
geometry.OgcGeometryType                    | CASE GeometryType(@geometry) WHEN 'POINT' THEN 1 ... END
geometry.Overlaps(g)                        | Overlaps(@geometry, @g)
geometry.PointOnSurface                     | PointOnSurface(@geometry)
geometry.Relate(g, intersectionPattern)     | Relate(@geometry, @g, @intersectionPattern)
geometry.Reverse()                          | ST_Reverse(@geometry)
geometry.SRID                               | SRID(@geometry)
geometry.SymmetricDifference(other)         | SymDifference(@geometry, @other)
geometry.ToBinary()                         | AsBinary(@geometry)
geometry.ToText()                           | AsText(@geometry)
geometry.Touches(g)                         | Touches(@geometry, @g)
geometry.Union()                            | UnaryUnion(@geometry)
geometry.Union(other)                       | GUnion(@geometry, @other)
geometry.Within(g)                          | Within(@geometry, @g)
geometryCollection[i]                       | GeometryN(@geometryCollection, @i + 1)
geometryCollection.Count                    | NumGeometries(@geometryCollection)
lineString.Count                            | NumPoints(@lineString)
lineString.EndPoint                         | EndPoint(@lineString)
lineString.GetPointN(n)                     | PointN(@lineString, @n + 1)
lineString.IsClosed                         | IsClosed(@lineString)
lineString.IsRing                           | IsRing(@lineString)
lineString.StartPoint                       | StartPoint(@lineString)
multiLineString.IsClosed                    | IsClosed(@multiLineString)
point.M                                     | M(@point)
point.X                                     | X(@point)
point.Y                                     | Y(@point)
point.Z                                     | Z(@point)
polygon.ExteriorRing                        | ExteriorRing(@polygon)
polygon.GetInteriorRingN(n)                 | InteriorRingN(@polygon, @n + 1)
polygon.NumInteriorRings                    | NumInteriorRing(@polygon)

### Aggregate functions

.NET                                                              | SQL                           | Added in
----------------------------------------------------------------- | ----------------------------- | --------
GeometryCombiner.Combine(group.Select(x => x.Property))           | Collect(Property)             | EF Core 7.0
ConvexHull.Create(group.Select(x => x.Property))                  | ConvexHull(Collect(Property)) | EF Core 7.0
UnaryUnionOp.Union(group.Select(x => x.Property))                 | GUnion(Property)              | EF Core 7.0
EnvelopeCombiner.CombineAsGeometry(group.Select(x => x.Property)) | Extent(Property)              | EF Core 7.0

## Additional resources

* [SpatiaLite Homepage](https://www.gaia-gis.it/fossil/libspatialite)
* [NetTopologySuite API Documentation](https://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.html)
