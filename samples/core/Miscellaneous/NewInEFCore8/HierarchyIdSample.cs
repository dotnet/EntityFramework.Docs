namespace NewInEfCore8;

public static class HierarchyIdSample
{
    public static async Task SQL_Server_HierarchyId()
    {
        PrintSampleName();

        await using var context = new FamilyTreeContext();
        await context.Database.EnsureDeletedAsync();

        context.LoggingEnabled = true;

        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.ChangeTracker.Clear();

        context.LoggingEnabled = false;
        Console.WriteLine();

        var level = 0;
        while (true)
        {
            #region GetLevel
            var generation = await context.Halflings.Where(halfling => halfling.PathFromPatriarch.GetLevel() == level).ToListAsync();
            #endregion

            if (!generation.Any())
            {
                break;
            }

            Console.Write($"Generation {level}: ");

            for (var i = 0; i < generation.Count; i++)
            {
                var halfling = generation[i];
                Console.Write($"{halfling.Name}");
                if (i < generation.Count - 1)
                {
                    Console.Write(", ");
                }
            }

            Console.WriteLine();

            level++;
        }

        Console.WriteLine();
        context.LoggingEnabled = true;

        var directAncestor = (await FindDirectAncestor("Bilbo"))!;
        Console.WriteLine();
        Console.WriteLine($"The direct ancestor of Bilbo is {directAncestor.Name}");

        #region FindDirectAncestor
        async Task<Halfling?> FindDirectAncestor(string name)
            => await context.Halflings
                .SingleOrDefaultAsync(
                    ancestor => ancestor.PathFromPatriarch == context.Halflings
                        .Single(descendent => descendent.Name == name).PathFromPatriarch
                        .GetAncestor(1));
        #endregion

        Console.WriteLine();
        var ancestors = await FindAllAncestors("Bilbo").AsNoTracking().ToListAsync();

        Console.WriteLine();
        Console.WriteLine("Ancestors of Bilbo are:");
        foreach (var halfling in ancestors)
        {
            Console.WriteLine($"  {halfling.Name}");
        }

        #region FindAllAncestors
        IQueryable<Halfling> FindAllAncestors(string name)
            => context.Halflings.Where(
                    ancestor => context.Halflings
                        .Single(
                            descendent =>
                                descendent.Name == name
                                && ancestor.Id != descendent.Id)
                        .PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
                .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel());
        #endregion

        Console.WriteLine();
        var directDescendents = await FindDirectDescendents("Mungo").AsNoTracking().ToListAsync();

        Console.WriteLine();
        Console.WriteLine("Direct descendents of Mungo:");
        foreach (var descendent in directDescendents)
        {
            Console.WriteLine($"  {descendent.Name}");
        }

        #region FindDirectDescendents
        IQueryable<Halfling> FindDirectDescendents(string name)
            => context.Halflings.Where(
                descendent => descendent.PathFromPatriarch.GetAncestor(1) == context.Halflings
                    .Single(ancestor => ancestor.Name == name).PathFromPatriarch);
        #endregion

        Console.WriteLine();
        var descendents = await FindAllDescendents("Mungo").AsNoTracking().ToListAsync();

        Console.WriteLine();
        Console.WriteLine("All descendents of Mungo:");
        foreach (var descendent in descendents)
        {
            Console.WriteLine($"  {descendent.Name}");
        }

        #region FindAllDescendents
        IQueryable<Halfling> FindAllDescendents(string name)
            => context.Halflings.Where(
                    descendent => descendent.PathFromPatriarch.IsDescendantOf(
                        context.Halflings
                            .Single(
                                ancestor =>
                                    ancestor.Name == name
                                    && descendent.Id != ancestor.Id)
                            .PathFromPatriarch))
                .OrderBy(descendent => descendent.PathFromPatriarch.GetLevel());
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine("All descendents of Ponto:");
        foreach (var descendent in await FindAllDescendents("Ponto").AsNoTracking().ToListAsync())
        {
            Console.WriteLine($"  {descendent.Name}");
        }

        var mungo = await context.Halflings.SingleAsync(halfling => halfling.Name == "Mungo");
        var ponto = await context.Halflings.SingleAsync(halfling => halfling.Name == "Ponto" && halfling.YearOfBirth == 1216);

        #region LongoAndDescendents
        var longoAndDescendents = await context.Halflings.Where(
                descendent => descendent.PathFromPatriarch.IsDescendantOf(
                    context.Halflings.Single(ancestor => ancestor.Name == "Longo").PathFromPatriarch))
            .ToListAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("Reparenting of Longo from Mungo to Ponto:");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region GetReparentedValue
        foreach (var descendent in longoAndDescendents)
        {
            descendent.PathFromPatriarch
                = descendent.PathFromPatriarch.GetReparentedValue(
                    mungo.PathFromPatriarch, ponto.PathFromPatriarch)!;
        }

        await context.SaveChangesAsync();
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine("All descendents of Mungo:");
        foreach (var descendent in await FindAllDescendents("Mungo").AsNoTracking().ToListAsync())
        {
            Console.WriteLine($"  {descendent.Name}");
        }

        Console.WriteLine();
        Console.WriteLine("All descendents of Ponto:");
        foreach (var descendent in await FindAllDescendents("Ponto").AsNoTracking().ToListAsync())
        {
            Console.WriteLine($"  {descendent.Name}");
        }

        Console.WriteLine();
        context.LoggingEnabled = true;

        var bilbo = await context.Halflings.SingleAsync(halfling => halfling.Name == "Bilbo");
        var frodo = await context.Halflings.SingleAsync(halfling => halfling.Name == "Frodo");

        var commonAncestor = (await FindCommonAncestor(bilbo, frodo))!;
        Console.WriteLine();
        Console.WriteLine($"The common ancestor of Bilbo and Frodo is {commonAncestor.Name}");

        #region FindCommonAncestor
        async Task<Halfling?> FindCommonAncestor(Halfling first, Halfling second)
            => await context.Halflings
                .Where(
                    ancestor => first.PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch)
                                && second.PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
                .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel())
                .FirstOrDefaultAsync();
        #endregion
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    #region Halfling
    public class Halfling
    {
        public Halfling(HierarchyId pathFromPatriarch, string name, int? yearOfBirth = null)
        {
            PathFromPatriarch = pathFromPatriarch;
            Name = name;
            YearOfBirth = yearOfBirth;
        }

        public int Id { get; private set; }
        public HierarchyId PathFromPatriarch { get; set; }
        public string Name { get; set; }
        public int? YearOfBirth { get; set; }
    }
    #endregion

    public class FamilyTreeContext : DbContext
    {
        public bool LoggingEnabled { get; set; }

        public DbSet<Halfling> Halflings => Set<Halfling>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(
                    @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0",
                    sqlServerOptionsBuilder => sqlServerOptionsBuilder.UseHierarchyId())
                .EnableSensitiveDataLogging()
                .LogTo(
                    s =>
                    {
                        if (LoggingEnabled)
                        {
                            Console.WriteLine(s);
                        }
                    }, LogLevel.Information);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public async Task Seed()
        {
            #region AddRangeAsync
            await AddRangeAsync(
                new Halfling(HierarchyId.Parse("/"), "Balbo", 1167),
                new Halfling(HierarchyId.Parse("/1/"), "Mungo", 1207),
                new Halfling(HierarchyId.Parse("/2/"), "Pansy", 1212),
                new Halfling(HierarchyId.Parse("/3/"), "Ponto", 1216),
                new Halfling(HierarchyId.Parse("/4/"), "Largo", 1220),
                new Halfling(HierarchyId.Parse("/5/"), "Lily", 1222),
                new Halfling(HierarchyId.Parse("/1/1/"), "Bungo", 1246),
                new Halfling(HierarchyId.Parse("/1/2/"), "Belba", 1256),
                new Halfling(HierarchyId.Parse("/1/3/"), "Longo", 1260),
                new Halfling(HierarchyId.Parse("/1/4/"), "Linda", 1262),
                new Halfling(HierarchyId.Parse("/1/5/"), "Bingo", 1264),
                new Halfling(HierarchyId.Parse("/3/1/"), "Rosa", 1256),
                new Halfling(HierarchyId.Parse("/3/2/"), "Polo"),
                new Halfling(HierarchyId.Parse("/4/1/"), "Fosco", 1264),
                new Halfling(HierarchyId.Parse("/1/1/1/"), "Bilbo", 1290),
                new Halfling(HierarchyId.Parse("/1/3/1/"), "Otho", 1310),
                new Halfling(HierarchyId.Parse("/1/5/1/"), "Falco", 1303),
                new Halfling(HierarchyId.Parse("/3/2/1/"), "Posco", 1302),
                new Halfling(HierarchyId.Parse("/3/2/2/"), "Prisca", 1306),
                new Halfling(HierarchyId.Parse("/4/1/1/"), "Dora", 1302),
                new Halfling(HierarchyId.Parse("/4/1/2/"), "Drogo", 1308),
                new Halfling(HierarchyId.Parse("/4/1/3/"), "Dudo", 1311),
                new Halfling(HierarchyId.Parse("/1/3/1/1/"), "Lotho", 1310),
                new Halfling(HierarchyId.Parse("/1/5/1/1/"), "Poppy", 1344),
                new Halfling(HierarchyId.Parse("/3/2/1/1/"), "Ponto", 1346),
                new Halfling(HierarchyId.Parse("/3/2/1/2/"), "Porto", 1348),
                new Halfling(HierarchyId.Parse("/3/2/1/3/"), "Peony", 1350),
                new Halfling(HierarchyId.Parse("/4/1/2/1/"), "Frodo", 1368),
                new Halfling(HierarchyId.Parse("/4/1/3/1/"), "Daisy", 1350),
                new Halfling(HierarchyId.Parse("/3/2/1/1/1/"), "Angelica", 1381));

            await SaveChangesAsync();
            #endregion
        }
    }
}
