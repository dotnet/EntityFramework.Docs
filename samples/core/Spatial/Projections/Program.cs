using System;
using NetTopologySuite.Geometries;

#region snippet_ProjectTo
var seattle = new Point(-122.333056, 47.609722) { SRID = 4326 };
var redmond = new Point(-122.123889, 47.669444) { SRID = 4326 };

// In order to get the distance in meters, we need to project to an appropriate
// coordinate system. In this case, we're using SRID 2855 since it covers the
// geographic area of our data
var distanceInDegrees = seattle.Distance(redmond);
var distanceInMeters = seattle.ProjectTo(2855).Distance(redmond.ProjectTo(2855));
#endregion
Console.WriteLine($"Degrees: {distanceInDegrees:N4}");
Console.WriteLine($"Meters:  {distanceInMeters:N0}");
