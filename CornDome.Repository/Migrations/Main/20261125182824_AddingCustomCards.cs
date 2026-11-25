using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main;

/// <inheritdoc />
public partial class _20261125182824_AddingCustomCards : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CustomCards",
            columns: table => new
            {
                CardId = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                UserId = table.Column<int>(type: "INTEGER", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                CardType = table.Column<int>(type: "INTEGER", nullable: false),
                Landscape = table.Column<int>(type: "INTEGER", nullable: true),
                Ability = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                Cost = table.Column<int>(type: "INTEGER", nullable: true),
                Attack = table.Column<int>(type: "INTEGER", nullable: true),
                Defense = table.Column<int>(type: "INTEGER", nullable: true),
                Author = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                Artist = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                Visibility = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 2),
                ApprovedBy = table.Column<int>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomCards", x => x.CardId);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CustomCards");
    }
}
