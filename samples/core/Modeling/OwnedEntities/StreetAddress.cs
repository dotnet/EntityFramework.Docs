using Microsoft.EntityFrameworkCore;

namespace EFModeling.OwnedEntities;

#region StreetAddress
[Owned]
public class StreetAddress
{
    public string Street { get; set; }
    public string City { get; set; }
}
#endregion