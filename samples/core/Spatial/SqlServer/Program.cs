using System;
using System.IO;
using System.Linq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using SqlServer.Models;

using (var context = new WideWorldImportersContext())
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    context.AddRange(
        new City { CityName = "Bellemondville", Location = new Point(-122.128822, 47.643703) { SRID = 4326 } },
        new Country
        {
            CountryName = "'Merica",
            Border = new Polygon(
                new LinearRing(
                    new[]
                    {
                        new Coordinate(-123.128822, 46.643703), new Coordinate(-121.128822, 46.643703),
                        new Coordinate(-121.128822, 48.643703), new Coordinate(-123.128822, 48.643703),
                        new Coordinate(-123.128822, 46.643703)
                    })) { SRID = 4326 }
        });

    context.SaveChanges();
}

var currentLocation = new Point(-122.128822, 47.643703) { SRID = 4326 };
using var db = new WideWorldImportersContext();
#region snippet_Distance
// Find the nearest city
var nearestCity = db.Cities
    .OrderBy(c => c.Location.Distance(currentLocation))
    .FirstOrDefault();
#endregion
Console.WriteLine($"Nearest city: {nearestCity.CityName}");
#region snippet_Contains
// Find the containing country
var currentCountry = db.Countries
    .FirstOrDefault(c => c.Border.Contains(currentLocation));
#endregion
Console.WriteLine($"Current country: {currentCountry.CountryName}");

// Find which states/provinces a route intersects
var route = new GeoJsonReader().Read<LineString>(File.ReadAllText("seattle-to-new-york.json"));
route.SRID = 4326;
var statePorvincesIntersected = (from s in db.StateProvinces
                                 where s.Border.Intersects(route)
                                 orderby s.Border.Distance(currentLocation)
                                 select s).ToList();
Console.WriteLine("States/provinces intersected:");
foreach (var state in statePorvincesIntersected)
{
    Console.WriteLine($"\t{state.StateProvinceName}");
}
