using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            System.Console.WriteLine("all done, type any key to exit [after keeping above log!]");
            System.Console.ReadKey();
        }
    }
}
