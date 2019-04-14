namespace TableSplitting
{
    #region DetailedOrder
    public class DetailedOrder : Order
    {
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public byte[] Version { get; set; }
    }
    #endregion
}
