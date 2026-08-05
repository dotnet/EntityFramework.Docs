using System.Collections.Generic;

namespace EFModeling.ComplexTypes;

#region Address
public record Address
{
    public required string Line1 { get; init; }
    public string? Line2 { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public required string PostCode { get; init; }
}
#endregion

#region CustomerOrders
public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // A required (non-nullable) complex property.
    public required Address Address { get; set; }

    // An optional (nullable) complex property.
    public Address? SecondaryAddress { get; set; }

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
#endregion

#region Distributor
public class Distributor
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // A collection of complex types, mapped to a single JSON column.
    public List<Address> ShippingCenters { get; set; } = new();
}
#endregion

#region GeoLocation
public class GeoLocation
{
    // All properties are optional, so this complex type has no required property
    // of its own to anchor an optional (nullable) usage.
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class Place
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // An optional complex property whose type has no required properties.
    public GeoLocation? Location { get; set; }
}
#endregion

