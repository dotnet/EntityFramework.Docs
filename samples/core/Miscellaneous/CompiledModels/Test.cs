using System.Linq;
using BenchmarkDotNet.Attributes;
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
