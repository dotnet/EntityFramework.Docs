using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Threading.Tasks;

namespace CompiledModels;

// This demonstrates the correct way to create value converters
// that work with compiled models
public static class ValueConverterSample
{
    public static async Task RunSample()
    {
        using var context = new SampleContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        // Create and save a model with converted values
        var model = new SampleEntity
        {
            IsActive = true,
            Status = SampleStatus.Active
        };

        context.SampleEntities.Add(model);
        await context.SaveChangesAsync();

        // Query the data back
        var retrieved = await context.SampleEntities.FirstAsync();
        Console.WriteLine($"IsActive: {retrieved.IsActive}, Status: {retrieved.Status}");
    }
}

public class SampleEntity
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public SampleStatus Status { get; set; }
}

public enum SampleStatus
{
    Inactive,
    Active,
    Pending
}

public class SampleContext : DbContext
{
    public DbSet<SampleEntity> SampleEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlite("Data Source=sample.db")
            .LogTo(Console.WriteLine);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Using the correct value converter that works with compiled models
        modelBuilder.Entity<SampleEntity>(b =>
        {
            b.Property(e => e.IsActive)
                .HasConversion(BooleanToCharConverter.Instance);
            
            b.Property(e => e.Status)
                .HasConversion<string>();
        });
    }
}

// This is the CORRECT way to create a value converter for compiled models
// All referenced methods must be public or internal, not private
public sealed class BooleanToCharConverter : ValueConverter<bool, char>
{
    public static readonly BooleanToCharConverter Instance = new();

    public BooleanToCharConverter()
        : base(v => ConvertToChar(v), v => ConvertToBoolean(v))
    {
    }

    // PUBLIC methods - this is required for compiled models
    public static char ConvertToChar(bool value)
    {
        return value ? 'Y' : 'N';
    }

    // PUBLIC methods - this is required for compiled models
    public static bool ConvertToBoolean(char value)
    {
        return value == 'Y';
    }
}

// This would be INCORRECT and cause compilation errors with compiled models:
/*
public sealed class IncorrectBooleanToCharConverter : ValueConverter<bool, char>
{
    public IncorrectBooleanToCharConverter()
        : base(v => ConvertToChar(v), v => ConvertToBoolean(v))
    {
    }

    // PRIVATE methods - this would cause CS0122 compilation errors with compiled models
    private static char ConvertToChar(bool value)
    {
        return value ? 'Y' : 'N';
    }

    private static bool ConvertToBoolean(char value)
    {
        return value == 'Y';
    }
}
*/