using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Microsoft.EntityFrameworkCore.Update;

#region snippet_CreateUserOperation
public class CreateUserOperation : MigrationOperation
{
    public string Name { get; set; }
    public string Password { get; set; }
}
#endregion

internal static class MigrationBuilderExtensions
{
    #region snippet_MigrationBuilderExtension
    public static OperationBuilder<CreateUserOperation> CreateUser(
        this MigrationBuilder migrationBuilder,
        string name,
        string password)
    {
        var operation = new CreateUserOperation { Name = name, Password = password };
        migrationBuilder.Operations.Add(operation);

        return new OperationBuilder<CreateUserOperation>(operation);
    }
    #endregion
}

#region snippet_MigrationsSqlGenerator
public class MyMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
{
    public MyMigrationsSqlGenerator(
        MigrationsSqlGeneratorDependencies dependencies,
        ICommandBatchPreparer commandBatchPreparer)
        : base(dependencies, commandBatchPreparer)
    {
    }

    protected override void Generate(
        MigrationOperation operation,
        IModel model,
        MigrationCommandListBuilder builder)
    {
        if (operation is CreateUserOperation createUserOperation)
        {
            Generate(createUserOperation, builder);
        }
        else
        {
            base.Generate(operation, model, builder);
        }
    }

    private void Generate(
        CreateUserOperation operation,
        MigrationCommandListBuilder builder)
    {
        var sqlHelper = Dependencies.SqlGenerationHelper;
        var stringMapping = Dependencies.TypeMappingSource.FindMapping(typeof(string));

        builder
            .Append("CREATE USER ")
            .Append(sqlHelper.DelimitIdentifier(operation.Name))
            .Append(" WITH PASSWORD = ")
            .Append(stringMapping.GenerateSqlLiteral(operation.Password))
            .AppendLine(sqlHelper.StatementTerminator)
            .EndCommand();
    }
}
#endregion

internal class CustomOperationContext : DbContext
{
    private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Sample;ConnectRetryCount=0";

    #region snippet_OnConfiguring
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseSqlServer(_connectionString)
            .ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>();
    #endregion
}
