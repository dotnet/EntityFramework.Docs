using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using SqlServer.Models;

using (var context = new WideWorldImportersContext())
{
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

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

    await context.SaveChangesAsync();
}

var currentLocation = new Point(-122.128822, 47.643703) { SRID = 4326 };
using var db = new WideWorldImportersContext();
#region snippet_Distance
// Find the nearest city
var nearestCity = await db.Cities
    .OrderBy(c => c.Location.Distance(currentLocation))
    .FirstOrDefaultAsync();
#endregion
Console.WriteLine($"Nearest city: {nearestCity.CityName}");
#region snippet_Contains
// Find the containing country
var currentCountry = await db.Countries
    .FirstOrDefaultAsync(c => c.Border.Contains(currentLocation));
#endregion
Console.WriteLine($"Current country: {currentCountry.CountryName}");

// Find which states/provinces a route intersects
var route = new GeoJsonReader().Read<LineString>(File.ReadAllText("seattle-to-new-york.json"));
route.SRID = 4326;
var statePorvincesIntersected = await (from s in db.StateProvinces
                                 where s.Border.Intersects(route)
                                 orderby s.Border.Distance(currentLocation)
                                 select s).ToListAsync();
Console.WriteLine("States/provinces intersected:");
foreach (var state in statePorvincesIntersected)
{
    Console.WriteLine($"\t{state.StateProvinceName}");
}
