using System;
using System.Linq;
using BenchmarkDotNet.Running;
using CompiledModelTest;

public class Program
{
    public static void Main(string[] args)
    {
        PrintSomeStuff();
        BenchmarkRunner.Run<Test>();
    }

    public static void PrintSomeStuff()
    {
        using var context = new BlogsContext();
        var model = context.Model;

        Console.WriteLine("Model has:");
        Console.WriteLine($"  {model.GetEntityTypes().Count()} entity types");
        Console.WriteLine($"  {model.GetEntityTypes().SelectMany(e => e.GetProperties()).Count()} properties");
        Console.WriteLine($"  {model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()).Count()} relationships");

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
