using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.QueryFilters
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            QueryFiltersBasicExample();
            QueryFiltersWithNavigationsExample();
            QueryFiltersWithRequiredNavigationExample();
            QueryFiltersUsingNavigationExample();
        }

        private static void QueryFiltersBasicExample()
        {
            using (var db = new BloggingContext("diego"))
            {
                if (db.Database.EnsureCreated())
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

                    db.SaveChanges();

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

                        andrewDb.SaveChanges();
                    }

                    db.Posts
                        .Where(
                            p => p.Title == "Caring for tropical fish"
                                 || p.Title == "Cat care 101")
                        .ToList()
                        .ForEach(p => db.Posts.Remove(p));

                    db.SaveChanges();
                }
            }

            using (var db = new BloggingContext("Diego"))
            {
                var blogs = db.Blogs
                    .Include(b => b.Posts)
                    .ToList();

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
                blogs = db.Blogs
                    .Include(b => b.Posts)
                    .IgnoreQueryFilters()
                    .ToList();
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

        private static void QueryFiltersWithNavigationsExample()
        {
            using (var animalContext = new AnimalContext())
            {
                animalContext.Database.EnsureDeleted();
                animalContext.Database.EnsureCreated();

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

        private static void QueryFiltersUsingNavigationExample()
        {
            using (var db = new FilteredBloggingContextRequired())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

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

                db.SaveChanges();
            }

            Console.WriteLine("Query filters using navigations demo");
            using (var db = new FilteredBloggingContextRequired())
            {
                #region QueriesNavigation
                var filteredBlogs = db.Blogs.ToList();
                #endregion
                var filteredBlogsInclude = db.Blogs.Include(b => b.Posts).ToList();
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
}
