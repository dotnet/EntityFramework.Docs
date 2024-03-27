using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CarsMigrations;

[DbContext(typeof(ConvertNullsSample.CarsContext))]
[Migration("20210927174004_Cars")]
public class Cars : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Person",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Person", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Cars",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OwnerId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Cars", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Cars_OwnerId",
            table: "Cars",
            column: "OwnerId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Cars");

        migrationBuilder.DropTable(
            name: "Person");
    }
}
