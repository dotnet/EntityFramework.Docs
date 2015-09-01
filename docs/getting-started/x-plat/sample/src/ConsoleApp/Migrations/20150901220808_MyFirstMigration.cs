using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace ConsoleApp.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    BlogId = table.Column<int>(isNullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(isNullable: true),
                    Url = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.BlogId);
                });
            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostId = table.Column<int>(isNullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BlogId = table.Column<int>(isNullable: false),
                    Content = table.Column<string>(isNullable: true),
                    Title = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Post_Blog_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blog",
                        principalColumn: "BlogId");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Post");
            migrationBuilder.DropTable("Blog");
        }
    }
}
