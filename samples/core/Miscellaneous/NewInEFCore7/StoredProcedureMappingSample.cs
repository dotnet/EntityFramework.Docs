namespace NewInEfCore7;

public static class StoredProcedureMappingSample
{
    public static void Inset_Update_and_Delete_using_stored_procedures_with_TPH()
    {
        Console.WriteLine($">>>> Sample: {nameof(Inset_Update_and_Delete_using_stored_procedures_with_TPH)}");
        Console.WriteLine();

        SprocMappingTest<TphDocumentsContext>();
    }

    public static void Inset_Update_and_Delete_using_stored_procedures_with_TPT()
    {
        Console.WriteLine($">>>> Sample: {nameof(Inset_Update_and_Delete_using_stored_procedures_with_TPT)}");
        Console.WriteLine();

        SprocMappingTest<TptDocumentsContext>();
    }

    public static void Inset_Update_and_Delete_using_stored_procedures_with_TPC()
    {
        Console.WriteLine($">>>> Sample: {nameof(Inset_Update_and_Delete_using_stored_procedures_with_TPC)}");
        Console.WriteLine();

        SprocMappingTest<TpcDocumentsContext>();
    }

    private static void SprocMappingTest<TContext>()
        where TContext : DocumentsContext, new()
    {
        using var context = new TContext();
        context.Database.EnsureDeleted();
        context.LoggingEnabled = true;
        context.Database.EnsureCreated();
        context.CreateStoredProcedures();

        context.LoggingEnabled = true;

        context.Seed();
        context.ChangeTracker.Clear();

        context.Documents
            .Include(document => ((Book)document).Authors)
            .Include(document => ((Magazine)document).Editor)
            .Load();

        context.RemoveRange(context.People.Local.Where(person => person.Contact.Address.City == "Chigley"));
        context.RemoveRange(context.Magazines.Local.Where(magazine => magazine.Title.Contains("Amstrad")));
        context.RemoveRange(context.Books.Local.Where(book => book.NumberOfPages < 200));

        // Comment this out for issue
        context.SaveChanges();

        foreach (var magazine in context.Magazines.Local)
        {
            magazine.CoverPrice += 1.0m;
        }

        foreach (var book in context.Books.Local)
        {
            book.Title += " (New Edition!)";
        }

        context.SaveChanges();

        foreach (var person in context.People.Local.Where(person => person.Contact.Address.Country == "UK"))
        {
            person.Name = "Dr. " + person.Name;
            person.Contact.Phone = "+44 " + person.Contact.Phone!.Substring(1);
            person.Contact.Address.Country = "United Kingdom";
        }

        context.SaveChanges();

        // try
        // {
        //     using var context2 = new TContext();
        //     context2.Books.Single(book => book.Title.StartsWith("Test")).Isbn = "Mod1";
        //
        //     context.Books.Local.Single(book => book.Title.StartsWith("Test", StringComparison.Ordinal)).Isbn = null;
        //     context.SaveChanges();
        //
        //     context2.SaveChanges();
        //
        // }
        // catch (DbUpdateConcurrencyException exception)
        // {
        //     Console.WriteLine($"Caught expected: " + exception.Message);
        // }

        try
        {
            using var context2 = new TContext();
            context2.People.Single(person => person.Name.StartsWith("Dr. Kent")).Name += ": Legend!";

            context.People.Local.Single(person => person.Name.StartsWith("Dr. Kent", StringComparison.Ordinal)).Name += ": Hero!";
            context.SaveChanges();

            context2.SaveChanges();

        }
        catch (DbUpdateConcurrencyException exception)
        {
            Console.WriteLine($"Caught expected: " + exception.Message);
        }

        Console.WriteLine();
    }

}
