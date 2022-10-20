using EFSaving.Basics;
using EFSaving.Transactions;

namespace EFSaving;

internal class Program
{
    private static void Main(string[] args)
    {
        Sample.Run();
        RelatedData.Sample.Run();
        CascadeDelete.Sample.Run();
        Concurrency.BasicSample.Run();
        Concurrency.ConflictResolutionSample.Run();
        ControllingTransaction.Run();
        ManagingSavepoints.Run();
        SharingTransaction.Run();
        ExternalDbTransaction.Run();
        Disconnected.Sample.Run();
    }
}
