using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main;

/// <inheritdoc />
public partial class _20261117175202_AddingDailyGuessingGame : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "dailyCard",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                CardId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_dailyCard", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "dailyCardGame",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                DailyCardId = table.Column<int>(type: "INTEGER", nullable: false),
                PlayerKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                GuessedLetters = table.Column<string>(type: "TEXT", maxLength: 26, nullable: true),
                GuessedAttributes = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                Mistakes = table.Column<int>(type: "INTEGER", nullable: false),
                IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                Won = table.Column<bool>(type: "INTEGER", nullable: false),
                Started = table.Column<DateTime>(type: "TEXT", nullable: false),
                Completed = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_dailyCardGame", x => x.Id);
                table.ForeignKey(
                    name: "FK_dailyCardGame_dailyCard_DailyCardId",
                    column: x => x.DailyCardId,
                    principalTable: "dailyCard",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_dailyCard_Date",
            table: "dailyCard",
            column: "Date",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_dailyCardGame_DailyCardId_PlayerKey",
            table: "dailyCardGame",
            columns: new[] { "DailyCardId", "PlayerKey" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "dailyCardGame");

        migrationBuilder.DropTable(
            name: "dailyCard");
    }
}
