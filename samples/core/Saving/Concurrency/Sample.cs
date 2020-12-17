using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Concurrency
{
    public class Sample
    {
        public static void Run()
        {
            // Ensure database is created and has a person in it
            using (var setupContext = new PersonContext())
            {
                setupContext.Database.EnsureDeleted();
                setupContext.Database.EnsureCreated();

                setupContext.People.Add(new Person { FirstName = "John", LastName = "Doe" });
                setupContext.SaveChanges();
            }

            #region ConcurrencyHandlingCode
            using var context = new PersonContext();
            // Fetch a person from database and change phone number
            var person = context.People.Single(p => p.PersonId == 1);
            person.PhoneNumber = "555-555-5555";

            // Change the person's name in the database to simulate a concurrency conflict
            context.Database.ExecuteSqlRaw(
                "UPDATE dbo.People SET FirstName = 'Jane' WHERE PersonId = 1");

            var saved = false;
            while (!saved)
            {
                try
                {
                    // Attempt to save changes to the database
                    context.SaveChanges();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is Person)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = databaseValues[property];

                                // TODO: decide which value should be written to database
                                // proposedValues[property] = <value to be saved>;
                            }

                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }
            #endregion
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

            [ConcurrencyCheck]
            public string FirstName { get; set; }

            [ConcurrencyCheck]
            public string LastName { get; set; }

            public string PhoneNumber { get; set; }
        }
    }
}
