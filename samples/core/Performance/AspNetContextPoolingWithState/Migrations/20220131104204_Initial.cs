using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Performance.AspNetContextPoolingWithState.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forecasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TemperatureC = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forecasts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Forecasts",
                columns: new[] { "Id", "Date", "Summary", "TemperatureC", "TenantId" },
                values: new object[] { 1, new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chilly", -17, 0 });

            migrationBuilder.InsertData(
                table: "Forecasts",
                columns: new[] { "Id", "Date", "Summary", "TemperatureC", "TenantId" },
                values: new object[] { 2, new DateTime(2022, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Balmy", 38, 0 });

            migrationBuilder.InsertData(
                table: "Forecasts",
                columns: new[] { "Id", "Date", "Summary", "TemperatureC", "TenantId" },
                values: new object[] { 3, new DateTime(2022, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sweltering", -7, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Forecasts");
        }
    }
}
