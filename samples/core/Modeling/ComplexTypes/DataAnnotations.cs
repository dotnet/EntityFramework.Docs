using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.ComplexTypes.DataAnnotations;

#region AddressAttribute
[ComplexType]
public record Address
{
    public required string Line1 { get; init; }
    public string? Line2 { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public required string PostCode { get; init; }
}
#endregion

// An entity referencing an attribute-decorated complex type maps it as a complex
// property without any further configuration in OnModelCreating.
public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required Address Address { get; set; }
    public List<Order> Orders { get; } = new();
}

public class Order
{
    public int Id { get; set; }
    public required string Contents { get; set; }
    public required Address ShippingAddress { get; set; }
    public required Address BillingAddress { get; set; }
    public Customer Customer { get; set; } = null!;
}
