using System.Collections.Generic;

namespace Cosmos.ModelBuilding;

#region Distributor
public class Distributor
{
    public int Id { get; set; }
    public string ETag { get; set; }
    public ICollection<StreetAddress> ShippingCenters { get; set; }
}
#endregion