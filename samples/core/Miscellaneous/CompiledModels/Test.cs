using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using CompiledModelTest;
using Microsoft.EntityFrameworkCore;

[SimpleJob(invocationCount: 1, targetCount: 50)]
public class Test
{
    [Benchmark]
    public async Task TimeToFirstQuery()
    {
        using var context = new BlogsContext();
        var results = await context.Set<Blog0000>().ToListAsync();
    }
}
