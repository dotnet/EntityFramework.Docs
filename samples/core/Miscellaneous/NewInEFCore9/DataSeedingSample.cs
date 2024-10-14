using NetTopologySuite.Operation.Valid;

namespace NewInEfCore9;

public static class DataSeedingSample
{
    public static async Task Data_seeding_in_EF9()
    {
        PrintSampleName();

        await using var context = new DataSeedingContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await using var secondContext = new DataSeedingContext();
        // seeding method gets invoked here also
        await context.Database.EnsureCreatedAsync();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
