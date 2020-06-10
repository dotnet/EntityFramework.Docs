using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Samples
{
    public class Program
    {
        private static void Main()
        {
            QueryFiltersWithNavigationsExample();

            QueryFiltersWithRequiredNavigationExample();
        }

        private static void QueryFiltersWithNavigationsExample()
        {
            SetupDatabase();

            using (var animalContext = new AnimalContext())
            {
                Console.WriteLine("*****************");
                Console.WriteLine("* Animal lovers *");
                Console.WriteLine("*****************");

                // Jamie and Paul are filtered out.
                // Paul doesn't own any pets. Jamie owns Puffy, but her pet has been filtered out.
                var animalLovers = animalContext.People.ToList();
                DisplayResults(animalLovers);

                Console.WriteLine("**************************************************");
                Console.WriteLine("* Animal lovers and their pets - filters enabled *");
                Console.WriteLine("**************************************************");

                // Jamie and Paul are filtered out.
                // Paul doesn't own any pets. Jamie owns Puffy, but her pet has been filtered out.
                // Simba's favorite toy has also been filtered out.
                // Puffy is filtered out so he doesn't show up as Hati's friend.
                var ownersAndTheirPets = animalContext.People
                    .Include(p => p.Pets)
                    .ThenInclude(p => ((Dog)p).FavoriteToy)
                    .ToList();

                DisplayResults(ownersAndTheirPets);

                Console.WriteLine("*********************************************************");
                Console.WriteLine("* Animal lovers and their pets - query filters disabled *");
                Console.WriteLine("*********************************************************");

                var ownersAndTheirPetsUnfiltered = animalContext.People
                    .IgnoreQueryFilters()
                    .Include(p => p.Pets)
                    .ThenInclude(p => ((Dog)p).FavoriteToy)
                    .ToList();

                DisplayResults(ownersAndTheirPetsUnfiltered);
            }
        }

        private static void SetupDatabase()
        {
            using (var animalContext = new AnimalContext())
            {
                if (animalContext.Database.EnsureCreated())
                {
                    var janice = new Person { Name = "Janice" };
                    var jamie = new Person { Name = "Jamie" };
                    var cesar = new Person { Name = "Cesar" };
                    var paul = new Person { Name = "Paul" };
                    var dominic = new Person { Name = "Dominic" };

                    var kibbles = new Cat { Name = "Kibbles", PrefersCardboardBoxes = false, Owner = janice };
                    var sammy = new Cat { Name = "Sammy", PrefersCardboardBoxes = true, Owner = janice };
                    var puffy = new Cat { Name = "Puffy", PrefersCardboardBoxes = true, Owner = jamie };
                    var hati = new Dog { Name = "Hati", FavoriteToy = new Toy { Name = "Squeeky duck" }, Owner = dominic, FriendsWith = puffy };
                    var simba = new Dog { Name = "Simba", FavoriteToy = new Toy { Name = "Bone" }, Owner = cesar, FriendsWith = sammy };
                    puffy.Tolerates = hati;
                    sammy.Tolerates = simba;

                    animalContext.People.AddRange(janice, jamie, cesar, paul, dominic);
                    animalContext.Animals.AddRange(kibbles, sammy, puffy, hati, simba);
                    animalContext.SaveChanges();
                }
            }
        }

        private static void DisplayResults(List<Person> people)
        {
            foreach (var person in people)
            {
                Console.WriteLine($"{person.Name}");
                if (person.Pets != null)
                {
                    foreach (var pet in person.Pets)
                    {
                        Console.Write($" - {pet.Name} [{pet.GetType().Name}] ");
                        if (pet is Cat cat)
                        {
                            Console.Write($"| Prefers cardboard boxes: {(cat.PrefersCardboardBoxes ? "Yes" : "No")} ");
                            Console.WriteLine($"| Tolerates: {(cat.Tolerates != null ? cat.Tolerates.Name : "No one")}");
                        }
                        else if (pet is Dog dog)
                        {
                            Console.Write($"| Favorite toy: {(dog.FavoriteToy != null ? dog.FavoriteToy.Name : "None")} ");
                            Console.WriteLine($"| Friend: {(dog.FriendsWith != null ? dog.FriendsWith.Name : "The Owner")}");
                        }
                    }
                }

                Console.WriteLine();
            }
        }

        private static void QueryFiltersWithRequiredNavigationExample()
        {
            using (var db = new FilteredBloggingContextRequired())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                #region SeedData
                db.Blogs.Add(
                    new Blog
                    {
                        Url = "http://sample.com/blogs/fish",
                        Posts = new List<Post>
                        {
                            new Post { Title = "Fish care 101" },
                            new Post { Title = "Caring for tropical fish" },
                            new Post { Title = "Types of ornamental fish" }
                        }
                    });

                db.Blogs.Add(
                    new Blog
                    {
                        Url = "http://sample.com/blogs/cats",
                        Posts = new List<Post>
                        {
                            new Post { Title = "Cat care 101" },
                            new Post { Title = "Caring for tropical cats" },
                            new Post { Title = "Types of ornamental cats" }
                        }
                    });
                #endregion

                db.SaveChanges();
            }

            Console.WriteLine("Use of required navigations to access entity with query filter demo");
            using (var db = new FilteredBloggingContextRequired())
            {

                #region Queries
                var allPosts = db.Posts.ToList();
                var allPostsWithBlogsIncluded = db.Posts.Include(p => p.Blog).ToList();
                #endregion

                if (allPosts.Count == allPostsWithBlogsIncluded.Count)
                {
                    Console.WriteLine($"Query filters set up correctly. Result count for both queries: {allPosts.Count}.");
                }
                else
                {
                    Console.WriteLine("Unexpected discrepancy due to query filters and required navigations interaction.");
                    Console.WriteLine($"All posts count: {allPosts.Count}.");
                    Console.WriteLine($"All posts with blogs included count: {allPostsWithBlogsIncluded.Count}.");
                }
            }
        }
    }

    #region QueryFiltersWithNavigations model
    public class AnimalContext : DbContext
    {
        private static readonly ILoggerFactory _loggerFactory
            = LoggerFactory.Create(
                builder =>
                {
                    builder
                        .AddFilter((category, level) =>
                            level == LogLevel.Information
                            && category.EndsWith("Connection", StringComparison.Ordinal))
                        .AddConsole();
                });

        public DbSet<Person> People { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Toy> Toys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=Demo.QueryFiltersNavigations;Trusted_Connection=True;ConnectRetryCount=0;")
                .UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cat>().HasOne(c => c.Tolerates).WithOne(d => d.FriendsWith).HasForeignKey<Cat>(c => c.ToleratesId);
            modelBuilder.Entity<Dog>().HasOne(d => d.FavoriteToy).WithOne(t => t.BelongsTo).HasForeignKey<Toy>(d => d.BelongsToId);

            modelBuilder.Entity<Person>().HasQueryFilter(p => p.Pets.Count > 0);
            modelBuilder.Entity<Animal>().HasQueryFilter(a => !a.Name.StartsWith("P"));
            modelBuilder.Entity<Toy>().HasQueryFilter(a => a.Name.Length > 5);

            // invalid - cycle in query filter definitions
            //modelBuilder.Entity<Animal>().HasQueryFilter(a => a.Owner.Name != "John"); 
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Animal> Pets { get; set; }
    }

    public abstract class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Person Owner { get; set; }
    }

    public class Cat : Animal
    {
        public bool PrefersCardboardBoxes { get; set; }

        public int? ToleratesId { get; set; }

        public Dog Tolerates { get; set; }
    }

    public class Dog : Animal
    {
        public Toy FavoriteToy { get; set; }
        public Cat FriendsWith { get; set; }
    }

    public class Toy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? BelongsToId { get; set; }
        public Dog BelongsTo { get; set; }
    }

    #endregion

    #region QueryFiltersWithRequiredNavigation model

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }

        public Blog Blog { get; set; }
    }

    public class FilteredBloggingContextRequired : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=Demo.QueryFiltersRequiredNavigations;Trusted_Connection=True;ConnectRetryCount=0;");
        }

        //incorrect setup - required navigation used to reference entity that has query filter defined, but no query filter for the entity on the other side of the navigation
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired();
        //    modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
        //}

        // correct setup #1 - optional navigation used to reference entity that has query filter defined
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired(false);
            modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
        }

        // correct setup #2 - required navigation used and query filters are applied for entities on both sides of the navigation, making sure results are consistent
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).IsRequired();
        //    modelBuilder.Entity<Blog>().HasQueryFilter(b => b.Url.Contains("fish"));
        //    modelBuilder.Entity<Post>().HasQueryFilter(p => p.Blog.Url.Contains("fish"));
        //}
    }
    #endregion
}
