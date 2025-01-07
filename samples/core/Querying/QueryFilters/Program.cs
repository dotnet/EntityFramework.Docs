using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.QueryFilters;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await QueryFiltersBasicExample();
        await QueryFiltersWithNavigationsExample();
        await QueryFiltersWithRequiredNavigationExample();
        await QueryFiltersUsingNavigationExample();
    }

    private static async Task QueryFiltersBasicExample()
    {
        using (var db = new BloggingContext("diego"))
        {
            if (await db.Database.EnsureCreatedAsync())
            {
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

                await db.SaveChangesAsync();

                using (var andrewDb = new BloggingContext("andrew"))
                {
                    andrewDb.Blogs.Add(
                        new Blog
                        {
                            Url = "http://sample.com/blogs/catfish",
                            Posts = new List<Post>
                            {
                                new Post { Title = "Catfish care 101" }, new Post { Title = "History of the catfish name" }
                            }
                        });

                    await andrewDb.SaveChangesAsync();
                }

                (await db.Posts
                    .Where(
                        p => p.Title == "Caring for tropical fish"
                             || p.Title == "Cat care 101")
                    .ToListAsync())
                    .ForEach(p => db.Posts.Remove(p));

                await db.SaveChangesAsync();
            }
        }

        using (var db = new BloggingContext("Diego"))
        {
            var blogs = await db.Blogs
                .Include(b => b.Posts)
                .ToListAsync();

            foreach (var blog in blogs)
            {
                Console.WriteLine(
                    $"{blog.Url,-33} [Tenant: {db.Entry(blog).Property("_tenantId").CurrentValue}]");

                foreach (var post in blog.Posts)
                {
                    Console.WriteLine($" - {post.Title,-30} [IsDeleted: {post.IsDeleted}]");
                }

                Console.WriteLine();
            }

            #region IgnoreFilters
            blogs = await db.Blogs
                .Include(b => b.Posts)
                .IgnoreQueryFilters()
                .ToListAsync();
            #endregion

            foreach (var blog in blogs)
            {
                Console.WriteLine(
                    $"{blog.Url,-33} [Tenant: {db.Entry(blog).Property("_tenantId").CurrentValue}]");

                foreach (var post in blog.Posts)
                {
                    Console.WriteLine($" - {post.Title,-30} [IsDeleted: {post.IsDeleted}]");
                }
            }
        }
    }

    private static async Task QueryFiltersWithNavigationsExample()
    {
        using (var animalContext = new AnimalContext())
        {
            await animalContext.Database.EnsureDeletedAsync();
            await animalContext.Database.EnsureCreatedAsync();

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
            await animalContext.SaveChangesAsync();
        }

        using (var animalContext = new AnimalContext())
        {
            Console.WriteLine("*****************");
            Console.WriteLine("* Animal lovers *");
            Console.WriteLine("*****************");

            // Jamie and Paul are filtered out.
            // Paul doesn't own any pets. Jamie owns Puffy, but her pet has been filtered out.
            var animalLovers = await animalContext.People.ToListAsync();
            DisplayResults(animalLovers);

            Console.WriteLine("**************************************************");
            Console.WriteLine("* Animal lovers and their pets - filters enabled *");
            Console.WriteLine("**************************************************");

            // Jamie and Paul are filtered out.
            // Paul doesn't own any pets. Jamie owns Puffy, but her pet has been filtered out.
            // Simba's favorite toy has also been filtered out.
            // Puffy is filtered out so he doesn't show up as Hati's friend.
            var ownersAndTheirPets = await animalContext.People
                .Include(p => p.Pets)
                .ThenInclude(p => ((Dog)p).FavoriteToy)
                .ToListAsync();

            DisplayResults(ownersAndTheirPets);

            Console.WriteLine("*********************************************************");
            Console.WriteLine("* Animal lovers and their pets - query filters disabled *");
            Console.WriteLine("*********************************************************");

            var ownersAndTheirPetsUnfiltered = await animalContext.People
                .IgnoreQueryFilters()
                .Include(p => p.Pets)
                .ThenInclude(p => ((Dog)p).FavoriteToy)
                .ToListAsync();

            DisplayResults(ownersAndTheirPetsUnfiltered);
        }

        static void DisplayResults(List<Person> people)
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
            }
        }
    }

    private static async Task QueryFiltersWithRequiredNavigationExample()
    {
        using (var db = new FilteredBloggingContextRequired())
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

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

            await db.SaveChangesAsync();
        }

        Console.WriteLine("Use of required navigations to access entity with query filter demo");
        using (var db = new FilteredBloggingContextRequired())
        {
            #region Queries
            var allPosts = await db.Posts.ToListAsync();
            var allPostsWithBlogsIncluded = await db.Posts.Include(p => p.Blog).ToListAsync();
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

    private static async Task QueryFiltersUsingNavigationExample()
    {
        using (var db = new FilteredBloggingContextRequired())
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            #region SeedDataNavigation
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

            db.Blogs.Add(
                new Blog
                {
                    Url = "http://sample.com/blogs/catfish",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Catfish care 101" }, new Post { Title = "History of the catfish name" }
                    }
                });
            #endregion

            await db.SaveChangesAsync();
        }

        Console.WriteLine("Query filters using navigations demo");
        using (var db = new FilteredBloggingContextRequired())
        {
            #region QueriesNavigation
            var filteredBlogs = await db.Blogs.ToListAsync();
            #endregion
            var filteredBlogsInclude = await db.Blogs.Include(b => b.Posts).ToListAsync();
            if (filteredBlogs.Count == 2
                && filteredBlogsInclude.Count == 2)
            {
                Console.WriteLine("Blogs without any Posts are also filtered out. Posts must contain 'fish' in title.");
                Console.WriteLine(
                    "Filters are applied recursively, so Blogs that do have Posts, but those Posts don't contain 'fish' in the title will also be filtered out.");
            }
        }
    }
}
