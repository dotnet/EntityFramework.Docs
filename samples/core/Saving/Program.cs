using EFSaving.Basics;
using EFSaving.Transactions;

namespace EFSaving;

class Program
{
    static void Main()
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
