using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace SqlServer.Models
{
    [Table("StateProvinces", Schema = "Application")]
    class StateProvince
    {
        public int StateProvinceID { get; set; }

        public string StateProvinceName { get; set; }

        public Geometry Border { get; set; }
    }
}
