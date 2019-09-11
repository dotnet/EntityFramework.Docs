using System.Threading.Tasks;

namespace Cosmos
{
    class Program
    {
        static async Task Main()
        {
            await ModelBuilding.Sample.Run();
            await UnstructuredData.Sample.Run();
        }
    }
}
