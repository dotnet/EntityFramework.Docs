using System;
using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

internal class PopulateCustomerFullName : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        #region snippet_RawSqlDataMigration
        migrationBuilder.AddColumn<string>(
            name: "FullName",
            table: "Customers",
            nullable: true);

        if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
        {
            migrationBuilder.Sql(
                """
                UPDATE [Customers]
                SET [FullName] = [FirstName] + N' ' + [LastName];
                """);
        }
        else if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            migrationBuilder.Sql(
                """
                UPDATE "Customers"
                SET "FullName" = "FirstName" || ' ' || "LastName";
                """);
        }
        else
        {
            throw new NotSupportedException(
                $"Data migration is not implemented for provider {migrationBuilder.ActiveProvider}.");
        }

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Customers",
            nullable: false,
            oldClrType: typeof(string),
            oldNullable: true);

        migrationBuilder.DropColumn(
            name: "FirstName",
            table: "Customers");

        migrationBuilder.DropColumn(
            name: "LastName",
            table: "Customers");
        #endregion
    }

    protected override void Down(MigrationBuilder migrationBuilder)
        => throw new NotSupportedException("Restore the original name columns from a backup before downgrading.");
}

internal class InsertCountries : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        #region snippet_InsertData
        migrationBuilder.InsertData(
            table: "Countries",
            columns: new[] { "CountryId", "Name" },
            values: new object[,]
            {
                { 1, "United States" },
                { 2, "Canada" }
            });
        #endregion
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Countries",
            keyColumn: "CountryId",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Countries",
            keyColumn: "CountryId",
            keyValue: 2);
    }
}

internal class RenameCountry : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        #region snippet_UpdateData
        migrationBuilder.UpdateData(
            table: "Countries",
            keyColumn: "CountryId",
            keyValue: 1,
            column: "Name",
            value: "United States of America");
        #endregion
    }

    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.UpdateData(
            table: "Countries",
            keyColumn: "CountryId",
            keyValue: 1,
            column: "Name",
            value: "United States");
}

internal class RemoveObsoleteCountry : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        #region snippet_DeleteData
        migrationBuilder.DeleteData(
            table: "Countries",
            keyColumn: "CountryId",
            keyValue: 2);
        #endregion
    }

    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.InsertData(
            table: "Countries",
            columns: new[] { "CountryId", "Name" },
            values: new object[] { 2, "Canada" });
}