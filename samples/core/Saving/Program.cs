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
            Transactions.ControllingTransaction.Sample.Run();
            Transactions.SharingTransaction.Sample.Run();
            Transactions.ExternalDbTransaction.Sample.Run();
            ExplicitValuesGenerateProperties.Sample.Run();
            Async.Sample.RunAsync().Wait();
            Disconnected.Sample.Run();
        }
    }
}
