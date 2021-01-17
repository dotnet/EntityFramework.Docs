using System;

namespace Samples.Model
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public int? PersonID { get; set; }
        public int? StoreID { get; set; }
        public int? TerritoryID { get; set; }
        public string AccountNumber { get; set; }
        public Guid rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
