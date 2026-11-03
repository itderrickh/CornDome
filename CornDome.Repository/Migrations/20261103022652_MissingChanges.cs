using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    /// <inheritdoc />
    public partial class MissingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_darklands_cardimage_darklands_card_DarklandsCardId",
                table: "darklands_cardimage");

            migrationBuilder.DropIndex(
                name: "IX_darklands_cardimage_DarklandsCardId",
                table: "darklands_cardimage");

            migrationBuilder.DropColumn(
                name: "DarklandsCardId",
                table: "darklands_cardimage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DarklandsCardId",
                table: "darklands_cardimage",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_darklands_cardimage_DarklandsCardId",
                table: "darklands_cardimage",
                column: "DarklandsCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_darklands_cardimage_darklands_card_DarklandsCardId",
                table: "darklands_cardimage",
                column: "DarklandsCardId",
                principalTable: "darklands_card",
                principalColumn: "Id");
        }
    }
}
