using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class CosmosImplicitOwnershipSample
{
    public static async Task Cosmos_models_use_implicit_ownership_by_default()
    {
        Console.WriteLine($">>>> Sample: {nameof(Cosmos_models_use_implicit_ownership_by_default)}");
        Console.WriteLine();

        using (var context = new FamilyContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.AddRange(
                new Family
                {
                    Id = "Andersen.1",
                    LastName = "Andersen",
                    Parents =
                    {
                        new() { FirstName = "Thomas" }, new() { FirstName = "Mary Kay" }
                    },
                    Children =
                    {
                        new()
                        {
                            FirstName = "Henriette Thaulow",
                            Gender = "female",
                            Grade = 5,
                            Pets = { new() { GivenName = "Fluffy" } }
                        }
                    },
                    Address = new Address { State = "WA", County = "King", City = "Seattle" },
                },
                new Family
                {
                    Id = "Wakefield.7",
                    LastName = "Wakefield",
                    Parents =
                    {
                        new() { FamilyName = "Wakefield", FirstName = "Robin" },
                        new() { FamilyName = "Miller", FirstName = "Ben" }
                    },
                    Children =
                    {
                        new()
                        {
                            FamilyName = "Merriam",
                            FirstName = "Jesse",
                            Gender = "female",
                            Grade = 8,
                            Pets = { new() { GivenName = "Goofy" }, new() { GivenName = "Shadow" } }
                        },
                        new Child
                        {
                            FamilyName = "Miller",
                            FirstName = "Lisa",
                            Gender = "female",
                            Grade = 1
                        }
                    },
                    Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                    IsRegistered = true
                });

            await context.SaveChangesAsync();
        }

        Console.WriteLine();

        using (var context = new FamilyContext())
        {
            var families = await context.Families.ToListAsync();

            Console.WriteLine();

            foreach (var family in families)
            {
                Console.WriteLine($"{family.LastName} family:");
                Console.WriteLine($"    From {family.Address.City}, {family.Address.State}");
                Console.WriteLine($"    With {family.Children.Count} children and {family.Children.SelectMany(e => e.Pets).Count()} pets.");
            }
        }

        Console.WriteLine();
    }

    #region Model
    public class Family
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public string LastName { get; set; }
        public bool IsRegistered { get; set; }

        public Address Address { get; set; }

        public IList<Parent> Parents { get; } = new List<Parent>();
        public IList<Child> Children { get; } = new List<Child>();
    }

    public class Parent
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
    }

    public class Child
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public int Grade { get; set; }

        public string Gender { get; set; }

        public IList<Pet> Pets { get; } = new List<Pet>();
    }
    #endregion

    public class Pet
    {
        public string GivenName { get; set; }
    }

    public class Address
    {
        public string State { get; set; }
        public string County { get; set; }
        public string City { get; set; }
    }

    public class FamilyContext : DbContext
    {
        public DbSet<Family> Families { get; set; }

        private readonly bool _quiet;

        public FamilyContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region OnModelCreating
            modelBuilder.Entity<Family>().HasPartitionKey(e => e.LastName);
            #endregion
        }

        // Never called; just for documentation.
        private void OldOnModelCreating(ModelBuilder modelBuilder)
        {
            #region OldOnModelCreating
            modelBuilder.Entity<Family>()
                .HasPartitionKey(e => e.LastName)
                .OwnsMany(f => f.Parents);

            modelBuilder.Entity<Family>()
                .OwnsMany(f => f.Children)
                .OwnsMany(c => c.Pets);

            modelBuilder.Entity<Family>()
                .OwnsOne(f => f.Address);
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "TODOImplicit");

            if (!_quiet)
            {
                optionsBuilder.LogTo(
                    Console.WriteLine, new[]
                    {
                        CosmosEventId.ExecutedCreateItem,
                        CosmosEventId.ExecutedDeleteItem,
                        CosmosEventId.ExecutedReadItem,
                        CosmosEventId.ExecutedReadNext,
                        CosmosEventId.ExecutedReplaceItem,
                    });
            }
        }
    }
}
