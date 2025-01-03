using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using CompiledModelTest;

public class Program
{
    public static async Task Main(string[] args)
    {
        await PrintSomeStuff();
        BenchmarkRunner.Run<Test>();
    }

    public static async Task PrintSomeStuff()
    {
        using var context = new BlogsContext();
        var model = context.Model;

        Console.WriteLine("Model has:");
        Console.WriteLine($"  {model.GetEntityTypes().Count()} entity types");
        Console.WriteLine($"  {model.GetEntityTypes().SelectMany(e => e.GetProperties()).Count()} properties");
        Console.WriteLine($"  {model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()).Count()} relationships");

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
