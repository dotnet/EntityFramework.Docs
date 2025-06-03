using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Concurrency;

public class BasicSample
{
    public static async Task Run()
    {
        // Ensure database is created and has a person in it
        using (var setupContext = new PersonContext())
        {
            await setupContext.Database.EnsureDeletedAsync();
            await setupContext.Database.EnsureCreatedAsync();

            setupContext.People.Add(new Person { FirstName = "John", LastName = "Doe" });
            setupContext.People.Add(new Person { FirstName = "Marie", LastName = "Jane" });
            await setupContext.SaveChangesAsync();
        }

        await SuccessfulUpdate();
        await ConcurrencyFailure();
    }

    // This shows a successful update, where no concurrent change happens
    private static async Task SuccessfulUpdate()
    {
        using var context = new PersonContext();

        var person = await context.People.SingleAsync(b => b.FirstName == "John");
        person.FirstName = "Paul";
        await context.SaveChangesAsync();

        Console.WriteLine("The change completed successfully.");
    }

    // This simulates a concurrency failure by modifying the row via another context after it was loaded.
    private static async Task ConcurrencyFailure()
    {
        using var context1 = new PersonContext();

        var person = await context1.People.SingleAsync(b => b.FirstName == "Marie");
        person.FirstName = "Stephanie";

        // We loaded the Blog instance - along with its concurrency token - and made a change on it.
        // Let's simulate a concurrent change by updating the row from another context
        using (var context2 = new PersonContext())
        {
            var person2 = await context2.People.SingleAsync(b => b.FirstName == "Marie");
            person2.FirstName = "Rachel";
            await context2.SaveChangesAsync();
        }

        // The tracked person in EF's change tracker has an out of date concurrency token, so calling SaveChanges will now throw
        // a DbUpdateConcurrencyException
        Console.WriteLine("SaveChanges should now throw:");
        await context1.SaveChangesAsync();
    }

    public class PersonContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Requires NuGet package Microsoft.EntityFrameworkCore.SqlServer
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Concurrency;Trusted_Connection=True;ConnectRetryCount=0");
        }
    }

    public class Person
    {
        public int PersonId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}
