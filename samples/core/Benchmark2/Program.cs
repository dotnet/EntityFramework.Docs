#define BM

#if BM
using BenchmarkDotNet.Running;
#endif

namespace Benchmark2
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if BM
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#else
            var sut = new FindRows();
            sut.Setup();
            sut.SqlServer();            // method decorated with [Benchmark], so benchmark.net will find & call directly
#endif
            System.Console.WriteLine("all done, type any key to exit [after keeping above log!]");
            System.Console.ReadKey();
        }
    }
}
