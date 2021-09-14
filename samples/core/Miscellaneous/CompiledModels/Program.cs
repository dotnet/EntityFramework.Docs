using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using CompiledModelTest;

[SimpleJob(invocationCount: 1, targetCount: 50)]
public class Test
{
    [Benchmark]
    public void TimeToFirstQuery()
    {
        using var context = new BlogsContext();
        var results = context.Set<Blog0000>().ToList();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        PrintSomeStuff();
        var summary = BenchmarkRunner.Run<Test>();
    }

    public static void PrintSomeStuff()
    {
        using var context = new BlogsContext();
        var model = context.Model;

        Console.WriteLine("Model has:");
        Console.WriteLine($"  {model.GetEntityTypes().Count()} entity types");
        Console.WriteLine($"  {model.GetEntityTypes().SelectMany(e => e.GetProperties()).Count()} properties");
        Console.WriteLine($"  {model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()).Count()} relationships");
    }
}
