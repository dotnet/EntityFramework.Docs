using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class GroupBySample
{
    public static async Task Translate_GroupBy_followed_by_FirstOrDefault_over_group()
    {
        Console.WriteLine($">>>> Sample: {nameof(Translate_GroupBy_followed_by_FirstOrDefault_over_group)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        // Example 1. From #12088
        using (var context = new ShoesContext())
        {
            #region GroupBy1
            var people = await context.People
                .Include(e => e.Shoes)
                .GroupBy(e => e.FirstName)
                .Select(
                    g => g.OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .FirstOrDefault())
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var person in people)
            {
                Console.WriteLine($"{person.FirstName} {person.MiddleInitial} {person.LastName} has {person.Shoes.Count} pairs of shoes.");
            }

            Console.WriteLine();
        }

        // Example 2. From #16648
        using (var context = new ShoesContext())
        {
            #region GroupBy2
            var group = await context.People
                .Select(
                    p => new
                    {
                        p.FirstName,
                        FullName = p.FirstName + " " + p.MiddleInitial + " " + p.LastName
                    })
                .GroupBy(p => p.FirstName)
                .Select(g => g.First())
                .FirstAsync();
            #endregion

            Console.WriteLine();
            Console.WriteLine($"First name: {group.FirstName} Full name: {group.FullName}");
            Console.WriteLine();
        }

        // Example 3. From #12640
        using (var context = new ShoesContext())
        {
            #region GroupBy3
            var people = await context.People
                .Where(e => e.MiddleInitial == "Q" && e.Age == 20)
                .GroupBy(e => e.LastName)
                .Select(g => g.First().LastName)
                .OrderBy(e => e.Length)
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var person in people)
            {
                Console.WriteLine(person);
            }

            Console.WriteLine();
        }

        // Example 4: From #18037
        using (var context = new ShoesContext())
        {
            #region GroupBy4
            var results = await (from person in context.People
                           join shoes in context.Shoes on person.Age equals shoes.Age
                           group shoes by shoes.Style
                           into people
                           select new
                           {
                               people.Key,
                               Style = people.Select(p => p.Style).FirstOrDefault(),
                               Count = people.Count()
                           })
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var result in results)
            {
                Console.WriteLine($"{result.Key}: {result.Style} Count: {result.Count}");
            }

            Console.WriteLine();
        }

        // Example 5. From #12601
        using (var context = new ShoesContext())
        {
            #region GroupBy5
            var results = await context.People
                .GroupBy(e => e.FirstName)
                .Select(g => g.First().LastName)
                .OrderBy(e => e)
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            Console.WriteLine();
        }

        // Example 6. From #12600
        using (var context = new ShoesContext())
        {
            #region GroupBy6
            var results = await context.People
                .Where(e => e.Age == 20)
                .GroupBy(e => e.Id)
                .Select(g => g.First().MiddleInitial)
                .OrderBy(e => e)
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            Console.WriteLine();
        }

        // Example 7. From #25460
        using (var context = new ShoesContext())
        {
            #region GroupBy7
            var size = 11;
            var results
                = await context.People
                    .Where(
                        p => p.Feet.Size == size
                             && p.MiddleInitial != null
                             && p.Feet.Id != 1)
                    .GroupBy(
                        p => new
                        {
                            p.Feet.Size,
                            p.Feet.Person.LastName
                        })
                    .Select(
                        g => new
                        {
                            g.Key.LastName,
                            g.Key.Size,
                            Min = g.Min(p => p.Feet.Size),
                        })
                    .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var result in results)
            {
                Console.WriteLine($"Last name = {result.LastName} Min = {result.Min} Size = {result.Size}");
            }

            Console.WriteLine();
        }

        // Example 8. From #24869
        using (var context = new ShoesContext())
        {
            #region GroupBy8
            var result = await context.People
                .Include(x => x.Shoes)
                .Include(x => x.Feet)
                .GroupBy(
                    x => new
                    {
                        x.Feet.Id,
                        x.Feet.Size
                    })
                .Select(
                    x => new
                    {
                        Key = x.Key.Id + x.Key.Size,
                        Count = x.Count(),
                        Sum = x.Sum(el => el.Id),
                        SumOver60 = x.Sum(el => el.Id) / (decimal)60,
                        TotalCallOutCharges = x.Sum(el => el.Feet.Size == 11 ? 1 : 0)
                    })
                .CountAsync();
            #endregion

            Console.WriteLine();
            Console.WriteLine($"Count = {result}");
            Console.WriteLine();
        }

        // Example 9. From #24591
        using (var context = new ShoesContext())
        {
            #region GroupBy9
            var results = await context.People
                .GroupBy(n => n.FirstName)
                .Select(g => new
                {
                    Feet = g.Key,
                    Total = g.Sum(n => n.Feet.Size)
                })
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var result in results)
            {
                Console.WriteLine($"{result.Feet}: {result.Total}");
            }

            Console.WriteLine();
        }

        // Sample 10. From #24695 closed as duplicate of #13805.
        using (var context = new ShoesContext())
        {
            #region GroupBy10
            var results = from Person person1
                              in from Person person2
                                     in context.People
                                 select person2
                          join Shoes shoes
                              in context.Shoes
                              on person1.Age equals shoes.Age
                          group shoes by
                              new
                              {
                                  person1.Id,
                                  shoes.Style,
                                  shoes.Age
                              }
                          into temp
                          select
                              new
                              {
                                  temp.Key.Id,
                                  temp.Key.Age,
                                  temp.Key.Style,
                                  Values = from t
                                               in temp
                                           select
                                               new
                                               {
                                                   t.Id,
                                                   t.Style,
                                                   t.Age
                                               }
                              };
            #endregion

            Console.WriteLine();

            await foreach (var result in results.AsAsyncEnumerable())
            {
                Console.WriteLine($"{result.Id}: {result.Age} year old {result.Style}");
                foreach (var value in result.Values)
                {
                    Console.WriteLine($"    {value.Id}: {value.Age} year old {value.Style}");
                }
            }

            Console.WriteLine();
        }

        // Sample 11. From #19506 closed as duplicate of #13805.
        using (var context = new ShoesContext())
        {
            #region GroupBy11
            var grouping = await context.People
                .GroupBy(i => i.LastName)
                .Select(g => new { LastName = g.Key, Count = g.Count() , First = g.FirstOrDefault(), Take = g.Take(2)})
                .OrderByDescending(e => e.LastName)
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var group in grouping)
            {
                Console.WriteLine($"LastName: {group.LastName} Count: {group.Count} First: {group.First.FirstName} {group.First.MiddleInitial} {group.First.LastName}");

                foreach (var person in group.Take)
                {
                    Console.WriteLine($"    {person.FirstName} {person.MiddleInitial} {person.LastName}");
                }
            }

            Console.WriteLine();
        }

        // Sample 12. From #13805
        using (var context = new ShoesContext())
        {
            #region GroupBy12
            var grouping = await context.People
                .Include(e => e.Shoes)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .GroupBy(e => e.FirstName)
                .Select(g => new { Name = g.Key, People = g.ToList()})
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var group in grouping)
            {
                foreach (var person in group.People)
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName} has {person.Shoes.Count} pairs of shoes.");
                }
            }

            Console.WriteLine();
        }

        // Sample 13. From #12088
        using (var context = new ShoesContext())
        {
            #region GroupBy13
            var grouping = await context.People
                .GroupBy(m => new {m.FirstName, m.MiddleInitial })
                .Select(am => new
                {
                    Key = am.Key,
                    Items = am.ToList()
                })
                .ToListAsync();
            #endregion

            Console.WriteLine();

            foreach (var group in grouping)
            {
                Console.WriteLine($"Group: {group.Key}");

                foreach (var person in group.Items)
                {
                    Console.WriteLine($"    {person.FirstName} {person.MiddleInitial} {person.LastName}");
                }
            }

            Console.WriteLine();
        }
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new ShoesContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
        {
            using var context = new ShoesContext(quiet: true);

            context.AddRange(
                new Person
                {
                    FirstName = "Jim",
                    MiddleInitial = "A",
                    LastName = "Bob",
                    Age = 20,
                    Feet = new Feet { Size = 11 },
                    Shoes = { new() { Style = "Sneakers", Age = 19 }, new() { Style = "Dress", Age = 20 } }
                },
                new Person
                {
                    FirstName = "Tom",
                    MiddleInitial = "A",
                    LastName = "Bob",
                    Age = 20,
                    Feet = new Feet { Size = 12 },
                    Shoes = { new() { Style = "Sneakers", Age = 21 }, new() { Style = "Dress", Age = 19 } }
                },
                new Person
                {
                    FirstName = "Ben",
                    MiddleInitial = "Q",
                    LastName = "Bob",
                    Age = 20,
                    Feet = new Feet { Size = 12 },
                    Shoes = { new() { Style = "Sneakers", Age = 20 }, new() { Style = "Dress", Age = 21 } }
                },
                new Person
                {
                    FirstName = "Jim",
                    MiddleInitial = "Q",
                    LastName = "Jon",
                    Age = 20,
                    Feet = new Feet { Size = 11 },
                    Shoes = { new() { Style = "Sneakers", Age = 19 }, new() { Style = "Dress", Age = 20 } }
                },
                new Person
                {
                    FirstName = "Tom",
                    MiddleInitial = "A",
                    LastName = "Jon",
                    Age = 21,
                    Feet = new Feet { Size = 11 },
                    Shoes = { new() { Style = "Sneakers", Age = 21 }, new() { Style = "Dress", Age = 19 } }
                },
                new Person
                {
                    FirstName = "Ben",
                    MiddleInitial = "A",
                    LastName = "Jon",
                    Age = 21,
                    Feet = new Feet { Size = 12 },
                    Shoes = { new() { Style = "Sneakers", Age = 20 }, new() { Style = "Dress", Age = 21 } }
                },
                new Person
                {
                    FirstName = "Jim",
                    MiddleInitial = "Q",
                    LastName = "Don",
                    Age = 21,
                    Feet = new Feet { Size = 12 },
                    Shoes = { new() { Style = "Sneakers", Age = 19 }, new() { Style = "Dress", Age = 20 } }
                },
                new Person
                {
                    FirstName = "Tom",
                    MiddleInitial = "Q",
                    LastName = "Don",
                    Age = 21,
                    Feet = new Feet { Size = 11 },
                    Shoes = { new() { Style = "Sneakers", Age = 21 }, new() { Style = "Dress", Age = 19 } }
                },
                new Person
                {
                    FirstName = "Ben",
                    MiddleInitial = "A",
                    LastName = "Don",
                    Age = 21,
                    Feet = new Feet { Size = 11 },
                    Shoes = { new() { Style = "Sneakers", Age = 20 }, new() { Style = "Dress", Age = 21 } }
                },
                new Person
                {
                    FirstName = "Jim",
                    MiddleInitial = "A",
                    LastName = "Zee",
                    Age = 21,
                    Feet = new Feet { Size = 12 },
                    Shoes = { new() { Style = "Sneakers", Age = 19 }, new() { Style = "Dress", Age = 20 } }
                },
                new Person
                {
                    FirstName = "Tom",
                    MiddleInitial = "Q",
                    LastName = "Zee",
                    Age = 21,
                    Feet = new Feet { Size = 12 },
                    Shoes = { new() { Style = "Sneakers", Age = 21 }, new() { Style = "Dress", Age = 19 } }
                },
                new Person
                {
                    FirstName = "Ben",
                    MiddleInitial = "Q",
                    LastName = "Zee",
                    Age = 21,
                    Feet = new Feet { Size = 11 },
                    Shoes = { new() { Style = "Sneakers", Age = 20 }, new() { Style = "Dress", Age = 21 } }
                });

            await context.SaveChangesAsync();
        }
    }

    #region Model
    public class Person
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public Feet Feet { get; set; }
        public ICollection<Shoes> Shoes { get; } = new List<Shoes>();
    }

    public class Shoes
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Style { get; set; }
        public Person Person { get; set; }
    }

    public class Feet
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public Person Person { get; set; }
    }
    #endregion

    public class ShoesContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Shoes> Shoes { get; set; }
        public DbSet<Feet> Feet { get; set; }

        private readonly bool _quiet;

        public ShoesContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feet>().HasOne(e => e.Person).WithOne(e => e.Feet).HasForeignKey<Feet>();
        }
    }
}
