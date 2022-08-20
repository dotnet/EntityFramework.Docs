using NewInEfCore7;

public class Program
{
    public static async Task Main()
    {
        await TpcInheritanceSample.Inheritance_with_TPH();
        await TpcInheritanceSample.Inheritance_with_TPT();
        await TpcInheritanceSample.Inheritance_with_TPC();
        await TpcInheritanceSample.Inheritance_with_TPC_using_HiLo();
        await TpcInheritanceSample.Inheritance_with_TPC_using_Identity();

        await ExecuteDeleteSample.ExecuteDelete();
        await ExecuteDeleteSample.ExecuteDeleteTpt();
        await ExecuteDeleteSample.ExecuteDeleteTpc();
        await ExecuteDeleteSample.ExecuteDeleteSqlite();

        await ExecuteUpdateSample.ExecuteUpdate();
        await ExecuteUpdateSample.ExecuteUpdateTpt();
        await ExecuteUpdateSample.ExecuteUpdateTpc();
        await ExecuteUpdateSample.ExecuteUpdateSqlite();
    }
}
