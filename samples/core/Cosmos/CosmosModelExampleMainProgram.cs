using System.Threading.Tasks;
using Cosmos.ModelBuilding;

namespace Cosmos
{
    public class CosmosModelExampleMainProgram
    {
        private static async Task Main()
        {
            await Sample.Run();
            await UnstructuredData.Sample.Run();
            System.Console.Write("hit any key to exit:  ");
            System.Console.ReadKey();
        }
    }
}
