namespace EFModeling.OwnedEntities;

#region DetailedOrder
public class DetailedOrder
{
    public int Id { get; set; }
    public OrderDetails OrderDetails { get; set; }
    public OrderStatus Status { get; set; }
}
#endregion