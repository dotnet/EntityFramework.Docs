namespace NewInEfCore7;

public static class TpcInheritanceSample
{
    public static Task Inheritance_with_TPH()
    {
        PrintSampleName();
        return ManyToManyTest<TphAnimalsContext>();
    }

    public static Task Inheritance_with_TPT()
    {
        PrintSampleName();
        return ManyToManyTest<TptAnimalsContext>();
    }

    public static Task Inheritance_with_TPC()
    {
        PrintSampleName();
        return ManyToManyTest<TpcAnimalsContext>();
    }

    public static Task Inheritance_with_TPC_using_HiLo()
    {
        PrintSampleName();
        return ManyToManyTest<TpcHiLoAnimalsContext>();
    }

    public static Task Inheritance_with_TPC_using_Identity()
    {
        PrintSampleName();
        return ManyToManyTest<TpcIdentityAnimalsContext>();
    }

    public static async Task ManyToManyTest<TContext>()
        where TContext : AnimalsContext, new()
    {
        using (var context = new TContext())
        {
            await context.Database.EnsureDeletedAsync();

            Console.WriteLine(context.Model.ToDebugString());
            Console.WriteLine();

            await context.Database.EnsureCreatedAsync();

            var catFood = new PetFood("Lily's Kitchen", LifeStage.Adult);
            var dogFood = new PetFood("Canagan", LifeStage.Adult);
            var hay = new FarmFood("Hay");
            var sushi = new HumanFood("Sushi", 670);

            var arthur = new Human("Arthur") { Food = sushi };
            var wendy = new Human("Wendy");
            var christi = new Human("Christi");

            var alice = new Cat("Alice", "MBA") { Vet = "Pengelly", Food = catFood, Humans = { arthur, wendy } };

            var mac = new Cat("Mac", "Preschool") { Vet = "Pengelly", Food = catFood, Humans = { arthur, wendy } };

            var toast = new Dog("Toast", "Mr. Squirrel") { Vet = "Pengelly", Food = dogFood, Humans = { arthur, wendy } };

            var clyde = new FarmAnimal("Clyde", "Equus africanus asinus") { Value = 100.0m, Food = hay };

            wendy.FavoriteAnimal = toast;
            arthur.FavoriteAnimal = alice;
            christi.FavoriteAnimal = clyde;

            await context.AddRangeAsync(wendy, arthur, christi, alice, mac, toast, clyde);
            await context.SaveChangesAsync();
        }

        Console.WriteLine();

        using (var context = new TContext())
        {
            Console.WriteLine("All foods:");
            foreach (var food in await context.Foods.ToListAsync())
            {
                Console.WriteLine($"  >> {food}");
            }

            Console.WriteLine();

            Console.WriteLine("All animals:");
            foreach (var animal in await context.Animals.ToListAsync())
            {
                Console.WriteLine($"  >> {animal}");
            }

            Console.WriteLine();

            Console.WriteLine("Only pets:");
            foreach (var pet in await context.Pets.ToListAsync())
            {
                Console.WriteLine($"  >> {pet}");
            }

            Console.WriteLine();

            Console.WriteLine("Only cats:");
            foreach (var cat in await context.Cats.ToListAsync())
            {
                Console.WriteLine($"  >> {cat}");
            }

            Console.WriteLine();

            Console.WriteLine("Make some changes and save to the database.");

            var baxter = context.Add(
                new Cat("Baxter", "BSc") { Vet = "Bothell Pet Hospital", Food = new HumanFood("Blueberry scones", 900) }).Entity;

            context.Add(new Human("Katie") { Pets = { baxter }, FavoriteAnimal = baxter });

            context.Remove(context.Animals.Local.Single(e => e.Name == "Christi"));
            context.Humans.Local.Single(e => e.Name == "Wendy").Food = new HumanFood("White pizza", 400);

            var sushi = await context.Foods.OfType<HumanFood>().SingleAsync(e => e.Name == "Sushi");
            sushi.Calories -= 100;

            await context.SaveChangesAsync();

            Console.WriteLine();
        }

        using (var context = new TContext())
        {
            Console.WriteLine("All animals including foods, pets, humans, and favorite animals:");
            foreach (var animal in await context.Animals
                         .AsNoTracking()
                         .Include(e => e.Food)
                         .Include(e => ((Human)e).Pets).ThenInclude(e => e.Food)
                         .Include(e => ((Pet)e).Humans).ThenInclude(e => e.Food)
                         .Include(e => ((Human)e).FavoriteAnimal).ThenInclude(e => e!.Food)
                         .ToListAsync())
            {
                Console.Write($"  >> {animal}");

                if (animal is Pet pet
                    && pet.Humans.Any())
                {
                    Console.WriteLine($" has humans {string.Join(", ", pet.Humans.Select(e => e.Name))}");
                }
                else if (animal is Human human
                         && human.Pets.Any())
                {
                    Console.WriteLine($" has pets {string.Join(", ", human.Pets.Select(e => e.Name))}");
                }
                else
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    #region AnimalsHierarchy
    public abstract class Animal
    {
        protected Animal(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public abstract string Species { get; }

        public Food? Food { get; set; }
    }

    public abstract class Pet : Animal
    {
        protected Pet(string name)
            : base(name)
        {
        }

        public string? Vet { get; set; }

        public ICollection<Human> Humans { get; } = new List<Human>();
    }

    public class FarmAnimal : Animal
    {
        public FarmAnimal(string name, string species)
            : base(name)
        {
            Species = species;
        }

        public override string Species { get; }

        [Precision(18, 2)]
        public decimal Value { get; set; }

        public override string ToString()
            => $"Farm animal '{Name}' ({Species}/{Id}) worth {Value:C} eats {Food?.ToString() ?? "<Unknown>"}";
    }

    public class Cat : Pet
    {
        public Cat(string name, string educationLevel)
            : base(name)
        {
            EducationLevel = educationLevel;
        }

        public string EducationLevel { get; set; }
        public override string Species => "Felis catus";

        public override string ToString()
            => $"Cat '{Name}' ({Species}/{Id}) with education '{EducationLevel}' eats {Food?.ToString() ?? "<Unknown>"}";
    }

    public class Dog : Pet
    {
        public Dog(string name, string favoriteToy)
            : base(name)
        {
            FavoriteToy = favoriteToy;
        }

        public string FavoriteToy { get; set; }
        public override string Species => "Canis familiaris";

        public override string ToString()
            => $"Dog '{Name}' ({Species}/{Id}) with favorite toy '{FavoriteToy}' eats {Food?.ToString() ?? "<Unknown>"}";
    }

    public class Human : Animal
    {
        public Human(string name)
            : base(name)
        {
        }

        public override string Species => "Homo sapiens";

        public Animal? FavoriteAnimal { get; set; }
        public ICollection<Pet> Pets { get; } = new List<Pet>();

        public override string ToString()
            => $"Human '{Name}' ({Species}/{Id}) with favorite animal '{FavoriteAnimal?.Name ?? "<Unknown>"}'" +
               $" eats {Food?.ToString() ?? "<Unknown>"}";
    }
    #endregion

    public abstract class Food
    {
        public Guid Id { get; set; }
    }

    public class PetFood : Food
    {
        public PetFood(string brand, LifeStage lifeStage)
        {
            Brand = brand;
            LifeStage = lifeStage;
        }

        public string Brand { get; set; }
        public LifeStage LifeStage { get; set; }

        public override string ToString()
            => $"Pet food by '{Brand}' ({Id}) for life stage {LifeStage}";
    }

    public enum LifeStage
    {
        Juvenile,
        Adult,
        Senior
    }

    public class HumanFood : Food
    {
        public HumanFood(string name, int calories)
        {
            Name = name;
            Calories = calories;
        }

        [Column("Name")]
        public string Name { get; set; }

        public int Calories { get; set; }

        public override string ToString()
            => $"{Name} ({Id}) with calories {Calories}";
    }

    public class FarmFood : Food
    {
        public FarmFood(string name)
        {
            Name = name;
        }

        [Column("Name")]
        public string Name { get; set; }

        public override string ToString()
            => $"{Name} ({Id})";
    }

    public class TphAnimalsContext : AnimalsContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region UseTphMappingStrategy
            modelBuilder.Entity<Animal>().UseTphMappingStrategy();
            #endregion

            modelBuilder.Entity<Food>().UseTpcMappingStrategy();
            base.OnModelCreating(modelBuilder);
        }
    }

    public class TptAnimalsContext : AnimalsContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region UseTptMappingStrategy
            modelBuilder.Entity<Animal>().UseTptMappingStrategy();
            #endregion

            modelBuilder.Entity<Food>().UseTpcMappingStrategy();
            base.OnModelCreating(modelBuilder);
        }
    }

    public class TpcAnimalsContext : AnimalsContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region UseTpcMappingStrategy
            modelBuilder.Entity<Animal>().UseTpcMappingStrategy();
            #endregion

            modelBuilder.Entity<Food>().UseTpcMappingStrategy();
            base.OnModelCreating(modelBuilder);
        }
    }

    public class TpcHiLoAnimalsContext : AnimalsContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseHiLo();
            modelBuilder.Entity<Animal>().UseTpcMappingStrategy();
            modelBuilder.Entity<Food>().UseTpcMappingStrategy();
            base.OnModelCreating(modelBuilder);
        }
    }

    public class TpcIdentityAnimalsContext : AnimalsContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>().UseTpcMappingStrategy();

            #region UsingIdentity
            modelBuilder.Entity<Cat>().ToTable("Cats", tb => tb.Property(e => e.Id).UseIdentityColumn(1, 4));
            modelBuilder.Entity<Dog>().ToTable("Dogs", tb => tb.Property(e => e.Id).UseIdentityColumn(2, 4));
            modelBuilder.Entity<FarmAnimal>().ToTable("FarmAnimals", tb => tb.Property(e => e.Id).UseIdentityColumn(3, 4));
            modelBuilder.Entity<Human>().ToTable("Humans", tb => tb.Property(e => e.Id).UseIdentityColumn(4, 4));
            #endregion

            modelBuilder.Entity<Food>().UseTpcMappingStrategy();

            base.OnModelCreating(modelBuilder);
        }
    }

    public abstract class AnimalsContext : DbContext
    {
        #region AnimalSets
        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<Pet> Pets => Set<Pet>();
        public DbSet<FarmAnimal> FarmAnimals => Set<FarmAnimal>();
        public DbSet<Cat> Cats => Set<Cat>();
        public DbSet<Dog> Dogs => Set<Dog>();
        public DbSet<Human> Humans => Set<Human>();
        #endregion

        public DbSet<Food> Foods => Set<Food>();
        public DbSet<PetFood> PetFoods => Set<PetFood>();
        public DbSet<FarmFood> FarmFoods => Set<FarmFood>();
        public DbSet<HumanFood> HumanFoods => Set<HumanFood>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0");

            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region AnimalsInModelBuilder
            modelBuilder.Entity<Animal>();
            modelBuilder.Entity<Pet>();
            modelBuilder.Entity<Cat>();
            modelBuilder.Entity<Dog>();
            modelBuilder.Entity<FarmAnimal>();
            modelBuilder.Entity<Human>();
            #endregion

            modelBuilder.Entity<FarmAnimal>().Property(e => e.Species);

            modelBuilder.Entity<Human>()
                .HasMany(e => e.Pets)
                .WithMany(e => e.Humans)
                .UsingEntity<Dictionary<object, string>>(
                    "PetsHumans",
                    r => r.HasOne<Pet>().WithMany().OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne<Human>().WithMany().OnDelete(DeleteBehavior.ClientCascade));
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
