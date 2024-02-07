namespace NewInEfCore9;

public static class QuerySample
{
    public static Task Query_improvements_in_EF9()
    {
        PrintSampleName();
        return QueryTest<JsonBlogsContext>();
    }

    public static Task Query_improvements_in_EF9_on_SQLite()
    {
        PrintSampleName();
        return QueryTest<JsonBlogsContextSqlite>();
    }

    private static async Task QueryTest<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();

        Console.WriteLine();
        Console.WriteLine("Default parameterization:");
        Console.WriteLine();

        _ = await GetPosts(1);

        #region DefaultParameterization
        async Task<List<Post>> GetPosts(int id)
            => await context.Posts
                .Where(
                    e => e.Title == ".NET Blog" && e.Id == id)
                .ToListAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("Force parameterization of a constant:");
        Console.WriteLine();

        _ = await GetPostsForceParameter(1);

        #region ForceParameter
        async Task<List<Post>> GetPostsForceParameter(int id)
            => await context.Posts
                .Where(
                    e => e.Title == EF.Parameter(".NET Blog") && e.Id == id)
                .ToListAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("Force constant:");
        Console.WriteLine();

        _ = await GetPostsForceConstant(1);

        #region ForceConstant
        async Task<List<Post>> GetPostsForceConstant(int id)
            => await context.Posts
                .Where(
                    e => e.Title == ".NET Blog" && e.Id == EF.Constant(id))
                .ToListAsync();
        #endregion
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
