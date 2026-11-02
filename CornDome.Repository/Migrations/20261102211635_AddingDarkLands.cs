using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddingDarkLands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "darklands_cardtype",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_darklands_cardtype", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "darklands_overlord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_darklands_overlord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "darklands_card",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Ability = table.Column<string>(type: "TEXT", nullable: true),
                    Attack = table.Column<int>(type: "INTEGER", nullable: true),
                    Defense = table.Column<int>(type: "INTEGER", nullable: true),
                    OverlordId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_darklands_card", x => x.Id);
                    table.ForeignKey(
                        name: "FK_darklands_card_darklands_cardtype_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "darklands_cardtype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_darklands_card_darklands_overlord_OverlordId",
                        column: x => x.OverlordId,
                        principalTable: "darklands_overlord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "darklands_cardimage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardImageTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    DarklandsCardId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_darklands_cardimage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_darklands_cardimage_cardImageType_CardImageTypeId",
                        column: x => x.CardImageTypeId,
                        principalTable: "cardImageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_darklands_cardimage_darklands_card_CardId",
                        column: x => x.CardId,
                        principalTable: "darklands_card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_darklands_cardimage_darklands_card_DarklandsCardId",
                        column: x => x.DarklandsCardId,
                        principalTable: "darklands_card",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_darklands_card_CardTypeId",
                table: "darklands_card",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_darklands_card_OverlordId",
                table: "darklands_card",
                column: "OverlordId");

            migrationBuilder.CreateIndex(
                name: "IX_darklands_cardimage_CardId",
                table: "darklands_cardimage",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_darklands_cardimage_CardImageTypeId",
                table: "darklands_cardimage",
                column: "CardImageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_darklands_cardimage_DarklandsCardId",
                table: "darklands_cardimage",
                column: "DarklandsCardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "darklands_cardimage");

            migrationBuilder.DropTable(
                name: "darklands_card");

            migrationBuilder.DropTable(
                name: "darklands_cardtype");

            migrationBuilder.DropTable(
                name: "darklands_overlord");
        }
    }
}
