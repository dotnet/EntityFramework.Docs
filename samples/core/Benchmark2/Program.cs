using BenchmarkDotNet.Running;

namespace Benchmark2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            //var sut = new FindRows();
            //sut.Setup();
            //sut.SqlServer();

            System.Console.WriteLine("all done, type any key to exit [after keeping above log!]");
            System.Console.ReadKey();
        }
    }
}
