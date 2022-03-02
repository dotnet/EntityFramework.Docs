using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace SqlServer.Models;

#region snippet_City
[Table("Cities", Schema = "Application")]
public class City
{
    public int CityID { get; set; }

    public string CityName { get; set; }

    public Point Location { get; set; }
}
#endregion