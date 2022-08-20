using NewInEfCore7;

public class Program
{
    public static void Main()
    {
        // await TpcInheritanceSample.Inheritance_with_TPH();
        // await TpcInheritanceSample.Inheritance_with_TPT();
        // await TpcInheritanceSample.Inheritance_with_TPC();
        // await TpcInheritanceSample.Inheritance_with_TPC_using_HiLo();
        //
        // // Currently not working: see https://github.com/dotnet/efcore/issues/28195
        // // await TpcInheritanceSample.Inheritance_with_TPC_using_Identity();
        //
        // await ExecuteDeleteSample.ExecuteDelete();
        // await ExecuteDeleteSample.ExecuteDeleteTpt();
        // await ExecuteDeleteSample.ExecuteDeleteTpc();
        // await ExecuteDeleteSample.ExecuteDeleteSqlite();
        //
        // await ExecuteUpdateSample.ExecuteUpdate();
        // await ExecuteUpdateSample.ExecuteUpdateTpt();
        // await ExecuteUpdateSample.ExecuteUpdateTpc();
        // await ExecuteUpdateSample.ExecuteUpdateSqlite();

        //StoredProcedureMappingSample.Inset_Update_and_Delete_using_stored_procedures_with_TPH();
        //StoredProcedureMappingSample.Inset_Update_and_Delete_using_stored_procedures_with_TPT();
        StoredProcedureMappingSample.Inset_Update_and_Delete_using_stored_procedures_with_TPC();
    }
}
