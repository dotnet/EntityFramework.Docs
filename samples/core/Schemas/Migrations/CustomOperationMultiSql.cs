using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

static class MultiSqlMigrationBuilderExtensions
{
    #region snippet_CustomOperationMultiSql
    public static OperationBuilder<SqlOperation> CreateUser(
        this MigrationBuilder migrationBuilder,
        string name,
        string password) =>
        migrationBuilder.ActiveProvider switch
        {
            "Npgsql.EntityFrameworkCore.PostgreSQL" => migrationBuilder
                                .Sql($"CREATE USER {name} WITH PASSWORD '{password}';"),
            "Microsoft.EntityFrameworkCore.SqlServer" => migrationBuilder
                                .Sql($"CREATE USER {name} WITH PASSWORD = '{password}';"),
            _ => throw new Exception("Unexpected provider."),
        };
    #endregion
}
