using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SyncMergedCardModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cardImage_revision_CardRevisionId",
                table: "cardImage");

            migrationBuilder.DropForeignKey(
                name: "FK_set_revision_CardRevisionId",
                table: "set");

            migrationBuilder.DropTable(
                name: "revision");

            migrationBuilder.DropIndex(
                name: "IX_set_CardRevisionId",
                table: "set");

            migrationBuilder.DropIndex(
                name: "IX_cardImage_CardRevisionId",
                table: "cardImage");

            migrationBuilder.DropColumn(
                name: "CardRevisionId",
                table: "set");

            migrationBuilder.DropColumn(
                name: "CardRevisionId",
                table: "cardImage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardRevisionId",
                table: "set",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardRevisionId",
                table: "cardImage",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "revision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    CardTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    LandscapeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Ability = table.Column<string>(type: "TEXT", nullable: true),
                    Attack = table.Column<int>(type: "INTEGER", nullable: true),
                    Cost = table.Column<int>(type: "INTEGER", nullable: true),
                    Defense = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    RevisionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Rulings = table.Column<string>(type: "TEXT", nullable: true),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_revision_cardType_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "cardType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_revision_card_CardId",
                        column: x => x.CardId,
                        principalTable: "card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_revision_landscape_LandscapeId",
                        column: x => x.LandscapeId,
                        principalTable: "landscape",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_set_CardRevisionId",
                table: "set",
                column: "CardRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_cardImage_CardRevisionId",
                table: "cardImage",
                column: "CardRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_revision_CardId",
                table: "revision",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_revision_CardTypeId",
                table: "revision",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_revision_LandscapeId",
                table: "revision",
                column: "LandscapeId");

            migrationBuilder.AddForeignKey(
                name: "FK_cardImage_revision_CardRevisionId",
                table: "cardImage",
                column: "CardRevisionId",
                principalTable: "revision",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_set_revision_CardRevisionId",
                table: "set",
                column: "CardRevisionId",
                principalTable: "revision",
                principalColumn: "Id");
        }
    }
}
