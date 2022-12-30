using System.Collections.Generic;

namespace EFModeling.OwnedEntities;

#region Distributor
public class Distributor
{
    public int Id { get; set; }
    public ICollection<StreetAddress> ShippingCenters { get; set; }
}
#endregion