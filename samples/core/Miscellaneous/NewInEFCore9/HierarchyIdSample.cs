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

        #region HierarchyIdQuery
        var daisy = await context.Halflings.SingleAsync(e => e.Name == "Daisy");
        #endregion

        #region HierarchyIdParse1
        var child1 = new Halfling(HierarchyId.Parse(daisy.PathFromPatriarch, 1), "Toast");
        var child2 = new Halfling(HierarchyId.Parse(daisy.PathFromPatriarch, 2), "Wills");
        #endregion

        #region HierarchyIdParse2
        var child1b = new Halfling(HierarchyId.Parse(daisy.PathFromPatriarch, 1, 5), "Toast");
        #endregion

        context.AddRange(child1, child2, child1b);

        await context.SaveChangesAsync();
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
