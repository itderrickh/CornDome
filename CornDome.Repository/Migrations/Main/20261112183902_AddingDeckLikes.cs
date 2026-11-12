using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main;

/// <inheritdoc />
public partial class _20261112183902_AddingDeckLikes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsUpvoted",
            table: "DeckComments");

        migrationBuilder.CreateTable(
            name: "DeckLikes",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                UserId = table.Column<int>(type: "INTEGER", nullable: false),
                DeckId = table.Column<int>(type: "INTEGER", nullable: false),
                Liked = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeckLikes", x => x.Id);
                table.ForeignKey(
                    name: "FK_DeckLikes_Decks_DeckId",
                    column: x => x.DeckId,
                    principalTable: "Decks",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_DeckLikes_User_UserId",
                    column: x => x.UserId,
                    principalTable: "User",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DeckLikes_DeckId",
            table: "DeckLikes",
            column: "DeckId");

        migrationBuilder.CreateIndex(
            name: "IX_DeckLikes_UserId",
            table: "DeckLikes",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DeckLikes");

        migrationBuilder.AddColumn<bool>(
            name: "IsUpvoted",
            table: "DeckComments",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);
    }
}
