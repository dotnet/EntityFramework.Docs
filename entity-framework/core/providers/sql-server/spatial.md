---
title: Microsoft SQL Server Database Provider - Spatial Data - EF Core
description: Using spatial data with the Entity Framework Core Microsoft SQL Server database provider
author: SamMonoRT
ms.date: 10/02/2020
uid: core/providers/sql-server/spatial
---
# Spatial Data in the SQL Server EF Core Provider

This page includes additional information about using spatial data with the Microsoft SQL Server database provider. For general information about using spatial data in EF Core, see the main [Spatial Data](xref:core/modeling/spatial) documentation.

## Geography or geometry

By default, spatial properties are mapped to `geography` columns in SQL Server. To use `geometry`, [configure the column type](xref:core/modeling/entity-properties#column-data-types) in your model.

## Geography polygon rings

When using the `geography` column type, SQL Server imposes additional requirements on the exterior ring (or shell) and interior rings (or holes). The exterior ring must be oriented counterclockwise and the interior rings clockwise. [NetTopologySuite](https://nettopologysuite.github.io/NetTopologySuite/) (NTS) validates this before sending values to the database.

## FullGlobe

SQL Server has a non-standard geometry type to represent the full globe when using the `geography` column type. It also has a way to represent polygons based on the full globe (without an exterior ring). Neither of these are supported by NTS.

> [!WARNING]
> FullGlobe and polygons based on it aren't supported by NTS.

## Curves

As mentioned in the main [Spatial Data](xref:core/modeling/spatial) documentation, NTS currently cannot represent curves. This means that you'll need to transform CircularString, CompoundCurve, and CurePolygon values using the [STCurveToLine](/sql/t-sql/spatial-geography/stcurvetoline-geography-data-type) method before using them in EF Core.

> [!WARNING]
> CircularString, CompoundCurve, and CurePolygon aren't supported by NTS.

## Spatial function mappings

This table shows which NTS members are translated into which SQL functions. Note that the translations vary depending on whether the column is of type geography or geometry.

.NET                                      | SQL (geography)                                              | SQL (geometry)                                               | Added in
----------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ | --------
EF.Functions.CurveToLine(geometry)        | @geometry.STCurveToLine()                                    | @geometry.STCurveToLine()                                    | EF Core 7.0
geometry.Area                             | @geometry.STArea()                                           | @geometry.STArea()
geometry.AsBinary()                       | @geometry.STAsBinary()                                       | @geometry.STAsBinary()
geometry.AsText()                         | @geometry.AsTextZM()                                         | @geometry.AsTextZM()
geometry.Boundary                         |                                                              | @geometry.STBoundary()
geometry.Buffer(distance)                 | @geometry.STBuffer(@distance)                                | @geometry.STBuffer(@distance)
geometry.Centroid                         |                                                              | @geometry.STCentroid()
geometry.Contains(g)                      | @geometry.STContains(@g)                                     | @geometry.STContains(@g)
geometry.ConvexHull()                     | @geometry.STConvexHull()                                     | @geometry.STConvexHull()
geometry.Crosses(g)                       |                                                              | @geometry.STCrosses(@g)
geometry.Difference(other)                | @geometry.STDifference(@other)                               | @geometry.STDifference(@other)
geometry.Dimension                        | @geometry.STDimension()                                      | @geometry.STDimension()
geometry.Disjoint(g)                      | @geometry.STDisjoint(@g)                                     | @geometry.STDisjoint(@g)
geometry.Distance(g)                      | @geometry.STDistance(@g)                                     | @geometry.STDistance(@g)
geometry.Envelope                         |                                                              | @geometry.STEnvelope()
geometry.EqualsTopologically(g)           | @geometry.STEquals(@g)                                       | @geometry.STEquals(@g)
geometry.GeometryType                     | @geometry.STGeometryType()                                   | @geometry.STGeometryType()
geometry.GetGeometryN(n)                  | @geometry.STGeometryN(@n + 1)                                | @geometry.STGeometryN(@n + 1)
geometry.InteriorPoint                    |                                                              | @geometry.STPointOnSurface()
geometry.Intersection(other)              | @geometry.STIntersection(@other)                             | @geometry.STIntersection(@other)
geometry.Intersects(g)                    | @geometry.STIntersects(@g)                                   | @geometry.STIntersects(@g)
geometry.IsEmpty                          | @geometry.STIsEmpty()                                        | @geometry.STIsEmpty()
geometry.IsSimple                         |                                                              | @geometry.STIsSimple()
geometry.IsValid                          | @geometry.STIsValid()                                        | @geometry.STIsValid()
geometry.IsWithinDistance(geom, distance) | @geometry.STDistance(@geom) <= @distance                     | @geometry.STDistance(@geom) <= @distance
geometry.Length                           | @geometry.STLength()                                         | @geometry.STLength()
geometry.NumGeometries                    | @geometry.STNumGeometries()                                  | @geometry.STNumGeometries()
geometry.NumPoints                        | @geometry.STNumPoints()                                      | @geometry.STNumPoints()
geometry.OgcGeometryType                  | CASE @geometry.STGeometryType() WHEN N'Point' THEN 1 ... END | CASE @geometry.STGeometryType() WHEN N'Point' THEN 1 ... END
geometry.Overlaps(g)                      | @geometry.STOverlaps(@g)                                     | @geometry.STOverlaps(@g)
geometry.PointOnSurface                   |                                                              | @geometry.STPointOnSurface()
geometry.Relate(g, intersectionPattern)   |                                                              | @geometry.STRelate(@g, @intersectionPattern)
geometry.SRID                             | @geometry.STSrid                                             | @geometry.STSrid
geometry.SymmetricDifference(other)       | @geometry.STSymDifference(@other)                            | @geometry.STSymDifference(@other)
geometry.ToBinary()                       | @geometry.STAsBinary()                                       | @geometry.STAsBinary()
geometry.ToText()                         | @geometry.AsTextZM()                                         | @geometry.AsTextZM()
geometry.Touches(g)                       |                                                              | @geometry.STTouches(@g)
geometry.Union(other)                     | @geometry.STUnion(@other)                                    | @geometry.STUnion(@other)
geometry.Within(g)                        | @geometry.STWithin(@g)                                       | @geometry.STWithin(@g)
geometryCollection[i]                     | @geometryCollection.STGeometryN(@i + 1)                      | @geometryCollection.STGeometryN(@i + 1)
geometryCollection.Count                  | @geometryCollection.STNumGeometries()                        | @geometryCollection.STNumGeometries()
lineString.Count                          | @lineString.STNumPoints()                                    | @lineString.STNumPoints()
lineString.EndPoint                       | @lineString.STEndPoint()                                     | @lineString.STEndPoint()
lineString.GetPointN(n)                   | @lineString.STPointN(@n + 1)                                 | @lineString.STPointN(@n + 1)
lineString.IsClosed                       | @lineString.STIsClosed()                                     | @lineString.STIsClosed()
lineString.IsRing                         |                                                              | @lineString.IsRing()
lineString.StartPoint                     | @lineString.STStartPoint()                                   | @lineString.STStartPoint()
multiLineString.IsClosed                  | @multiLineString.STIsClosed()                                | @multiLineString.STIsClosed()
point.M                                   | @point.M                                                     | @point.M
point.X                                   | @point.Long                                                  | @point.STX
point.Y                                   | @point.Lat                                                   | @point.STY
point.Z                                   | @point.Z                                                     | @point.Z
polygon.ExteriorRing                      | @polygon.RingN(1)                                            | @polygon.STExteriorRing()
polygon.GetInteriorRingN(n)               | @polygon.RingN(@n + 2)                                       | @polygon.STInteriorRingN(@n + 1)
polygon.NumInteriorRings                  | @polygon.NumRings() - 1                                      | @polygon.STNumInteriorRing()

### Aggregate functions

.NET                                                              | SQL                           | Added in
----------------------------------------------------------------- | ----------------------------- | --------
GeometryCombiner.Combine(group.Select(x => x.Property))           | CollectionAggregate(Property) | EF Core 7.0
ConvexHull.Create(group.Select(x => x.Property))                  | ConvexHullAggregate(Property) | EF Core 7.0
UnaryUnionOp.Union(group.Select(x => x.Property))                 | UnionAggregate(Property)      | EF Core 7.0
EnvelopeCombiner.CombineAsGeometry(group.Select(x => x.Property)) | EnvelopeAggregate(Property)   | EF Core 7.0

## Additional resources

* [Spatial Data in SQL Server](/sql/relational-databases/spatial/spatial-data-sql-server)
* [NetTopologySuite Docs](https://nettopologysuite.github.io/NetTopologySuite/)
