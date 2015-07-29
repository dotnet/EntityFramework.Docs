using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Migrations.Operations;

namespace EFGetStartedUWPMigrations
{
    public partial class MyFirstMigration : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    BlogId = table.Column(type: "INTEGER", nullable: false), // <- Add this comma
                        //.Annotation("Sqlite:Autoincrement", "true"), // <- Remove or comment this line
                    Url = table.Column(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.BlogId);
                });
            migration.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostId = table.Column(type: "INTEGER", nullable: false), // <- Add this comma
                        //.Annotation("Sqlite:Autoincrement", "true"), // <- Remove or comment this line
                    BlogId = table.Column(type: "INTEGER", nullable: false),
                    Content = table.Column(type: "TEXT", nullable: true),
                    Title = table.Column(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Post_Blog_BlogId",
                        columns: x => x.BlogId,
                        referencedTable: "Blog",
                        referencedColumn: "BlogId");
                });
        }

        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("Post");
            migration.DropTable("Blog");
        }
    }
}
