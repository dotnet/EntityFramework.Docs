using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

internal static class MultiSqlMigrationBuilderExtensions
{
    #region snippet_CustomOperationMultiSql
    private static OperationBuilder<SqlOperation> CreateUser(
        this MigrationBuilder migrationBuilder,
        string name,
        string password)
    {
        switch (migrationBuilder.ActiveProvider)
        {
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                return migrationBuilder
                    .Sql($"CREATE USER {name} WITH PASSWORD '{password}';");

            case "Microsoft.EntityFrameworkCore.SqlServer":
                return migrationBuilder
                    .Sql($"CREATE USER {name} WITH PASSWORD = '{password}';");
        }

        throw new Exception("Unexpected provider.");
    }
    #endregion
}
