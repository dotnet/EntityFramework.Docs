using System.Threading.Tasks;
using EFSaving.Basics;
using EFSaving.Transactions;

namespace EFSaving;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await Sample.Run();
        await RelatedData.Sample.Run();
        await CascadeDelete.Sample.Run();
        await Concurrency.BasicSample.Run();
        await Concurrency.ConflictResolutionSample.Run();
        await ControllingTransaction.Run();
        await ManagingSavepoints.Run();
        await SharingTransaction.Run();
        await ExternalDbTransaction.Run();
        await Disconnected.Sample.Run();
    }
}
