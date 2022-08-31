namespace NewInEfCore7;

public static class StoredProcedureMappingSample
{
    public static Task Insert_Update_and_Delete_using_stored_procedures_with_TPH()
    {
        PrintSampleName();
        return SprocMappingTest<TphDocumentsContext>();
    }

    public static Task Insert_Update_and_Delete_using_stored_procedures_with_TPT()
    {
        PrintSampleName();
        return SprocMappingTest<TptDocumentsContext>();
    }

    public static Task Insert_Update_and_Delete_using_stored_procedures_with_TPC()
    {
        PrintSampleName();
        return SprocMappingTest<TpcDocumentsContext>();
    }

    private static async Task SprocMappingTest<TContext>()
        where TContext : DocumentsContext, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        context.LoggingEnabled = true;
        await context.Database.EnsureCreatedAsync();
        await context.CreateStoredProcedures();

        context.LoggingEnabled = true;

        await context.Seed();
        context.ChangeTracker.Clear();

        await context.Documents
            .Include(document => ((Book)document).Authors)
            .Include(document => ((Magazine)document).Editor)
            .LoadAsync();

        context.RemoveRange(context.People.Local.Where(person => person.Contact.Address.City == "Chigley"));
        context.RemoveRange(context.Magazines.Local.Where(magazine => magazine.Title.Contains("Amstrad")));
        context.RemoveRange(context.Books.Local.Where(book => book.NumberOfPages < 200));

        foreach (var magazine in context.Magazines.Local)
        {
            magazine.CoverPrice += 1.0m;
        }

        foreach (var book in context.Books.Local)
        {
            book.Title += " (New Edition!)";
        }

        foreach (var person in context.People.Local.Where(person => person.Contact.Address.Country == "UK"))
        {
            person.Name = "Dr. " + person.Name;
            person.Contact.Phone = "+44 " + person.Contact.Phone!.Substring(1);
            person.Contact.Address.Country = "United Kingdom";
        }

        // Only fails for TPH now:
        // https://github.com/dotnet/efcore/issues/28803 (Exception: "Unable to cast object of type" in many stored procs mapping cases)
        await context.SaveChangesAsync();

        // https://github.com/dotnet/efcore/issues/28803 (Exception: "Unable to cast object of type" in many stored procs mapping cases)
        // try
        // {
        //     await using var context2 = new TContext();
        //     (await context2.Books.SingleAsync(book => book.Title.StartsWith("Test"))).Isbn = "Mod1";
        //
        //     context.Books.Local.Single(book => book.Title.StartsWith("Test", StringComparison.Ordinal)).Isbn = null;
        //     await context.SaveChangesAsync();
        //
        //     await context2.SaveChangesAsync();
        //
        // }
        // catch (DbUpdateConcurrencyException exception)
        // {
        //     Console.WriteLine($"Caught expected: " + exception.Message);
        // }

        try
        {
            await using var context2 = new TContext();
            (await context2.People.SingleAsync(person => person.Name.StartsWith("Dr. Kent"))).Name += ": Legend!";

            context.People.Local.Single(person => person.Name.StartsWith("Dr. Kent", StringComparison.Ordinal)).Name += ": Hero!";
            await context.SaveChangesAsync();

            await context2.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException exception)
        {
            Console.WriteLine($"Caught expected: " + exception.Message);
        }

        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
