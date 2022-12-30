using Microsoft.EntityFrameworkCore;

namespace EFModeling.Seqiemces.SequenceConfiguration;

internal class MyContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    #region SequenceConfiguration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<int>("OrderNumbers", schema: "shared")
            .StartsAt(1000)
            .IncrementsBy(5);
    }
    #endregion
}

public class Order
{
    public int OrderId { get; set; }
    public int OrderNo { get; set; }
    public string Url { get; set; }
}