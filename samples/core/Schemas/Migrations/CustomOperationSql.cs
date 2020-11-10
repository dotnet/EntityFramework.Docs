using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

static class SqlMigrationBuilderExtensions
{
    #region snippet_CustomOperationSql
    static OperationBuilder<SqlOperation> CreateUser(
        this MigrationBuilder migrationBuilder,
        string name,
        string password)
        => migrationBuilder.Sql($"CREATE USER {name} WITH PASSWORD '{password}';");
    #endregion
}