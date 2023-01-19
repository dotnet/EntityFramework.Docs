namespace NewInEfCore8;

public static class InheritanceSample
{
    public static Task Discriminator_length_TPH()
    {
        PrintSampleName();
        return DiscriminatorLengthTest<TphDocumentsContext>();
    }
    private static async Task DiscriminatorLengthTest<TContext>()
        where TContext : DocumentsContext, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        context.LoggingEnabled = true;

        Console.WriteLine("Creating database tables...");
        Console.WriteLine();

        await context.Database.EnsureCreatedAsync();

        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
