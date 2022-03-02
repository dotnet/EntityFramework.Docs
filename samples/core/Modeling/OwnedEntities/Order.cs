namespace EFModeling.OwnedEntities;

#region Order
public class Order
{
    public int Id { get; set; }
    public StreetAddress ShippingAddress { get; set; }
}
#endregion