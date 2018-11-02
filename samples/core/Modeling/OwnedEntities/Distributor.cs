using System.Collections.Generic;

namespace OwnedEntities
{
    public class Distributor
    {
        public int Id { get; set; }
        public ICollection<StreetAddress> ShippingCenters { get; set; }
    }
}
