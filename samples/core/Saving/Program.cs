namespace EFSaving
{
    class Program
    {
        static void Main(string[] args)
        {
            Basics.Sample.Run();
            RelatedData.Sample.Run();
            CascadeDelete.Sample.Run();
            Concurrency.Sample.Run();
            Transactions.ControllingTransaction.Run();
            Transactions.ManagingSavepoints.Run();
            Transactions.SharingTransaction.Run();
            Transactions.ExternalDbTransaction.Run();
            ExplicitValuesGenerateProperties.Sample.Run();
            Disconnected.Sample.Run();
        }
    }
}
