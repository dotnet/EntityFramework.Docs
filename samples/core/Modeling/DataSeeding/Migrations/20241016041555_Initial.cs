using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EFModeling.DataSeeding.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details_Phonetic = table.Column<bool>(type: "bit", nullable: true),
                    Details_Tonal = table.Column<bool>(type: "bit", nullable: true),
                    Details_PhonemesCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocatedInId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cites_Countries_LocatedInId",
                        column: x => x.LocatedInId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageCountry",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageCountry", x => new { x.LanguageId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_LanguageCountry_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguageCountry_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            #region CustomInsert
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, "USA" },
                    { 2, "Canada" },
                    { 3, "Mexico" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Name", "Details_PhonemesCount", "Details_Phonetic", "Details_Tonal" },
                values: new object[,]
                {
                    { 1, "English", 44, false, false },
                    { 2, "French", 36, false, false },
                    { 3, "Spanish", 24, true, false }
                });

            migrationBuilder.InsertData(
                table: "Cites",
                columns: new[] { "Id", "LocatedInId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Seattle" },
                    { 2, 2, "Vancouver" },
                    { 3, 3, "Mexico City" },
                    { 4, 3, "Puebla" }
                });

            migrationBuilder.InsertData(
                table: "LanguageCountry",
                columns: new[] { "CountryId", "LanguageId" },
                values: new object[,]
                {
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });
            #endregion

            migrationBuilder.CreateIndex(
                name: "IX_Cites_LocatedInId",
                table: "Cites",
                column: "LocatedInId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageCountry_CountryId",
                table: "LanguageCountry",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cites");

            migrationBuilder.DropTable(
                name: "LanguageCountry");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
