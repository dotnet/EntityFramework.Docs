using System.Threading.Tasks;

namespace EFSaving
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Basics.Sample.RunAsync();
            await RelatedData.Sample.RunAsync();
            await CascadeDelete.Sample.RunAsync();
            await Concurrency.Sample.RunAsync();
            await Transactions.ControllingTransaction.RunAsync();
            await Transactions.ManagingSavepoints.RunAsync();
            await Transactions.SharingTransaction.RunAsync();
            await Transactions.ExternalDbTransaction.RunAsync();
            await ExplicitValuesGenerateProperties.Sample.RunAsync();
            await Disconnected.Sample.RunAsync();
        }
    }
}
