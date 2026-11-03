using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cardImageType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descriptor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardImageType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cardType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardType", x => x.Id);
                });

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
                name: "landscape",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_landscape", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "set",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_set", x => x.Id);
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
                name: "card",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsCustomCard = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ability = table.Column<string>(type: "TEXT", nullable: true),
                    LandscapeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Cost = table.Column<int>(type: "INTEGER", nullable: true),
                    Attack = table.Column<int>(type: "INTEGER", nullable: true),
                    Defense = table.Column<int>(type: "INTEGER", nullable: true),
                    Rulings = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_card", x => x.Id);
                    table.ForeignKey(
                        name: "FK_card_cardType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "cardType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_card_landscape_LandscapeId",
                        column: x => x.LandscapeId,
                        principalTable: "landscape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "darklands_cardimage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardImageTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "cardImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    CardImageTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cardImage_cardImageType_CardImageTypeId",
                        column: x => x.CardImageTypeId,
                        principalTable: "cardImageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cardImage_card_CardId",
                        column: x => x.CardId,
                        principalTable: "card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cardSet",
                columns: table => new
                {
                    CardSetsId = table.Column<int>(type: "INTEGER", nullable: false),
                    CardsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardSet", x => new { x.CardSetsId, x.CardsId });
                    table.ForeignKey(
                        name: "FK_cardSet_card_CardsId",
                        column: x => x.CardsId,
                        principalTable: "card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cardSet_set_CardSetsId",
                        column: x => x.CardSetsId,
                        principalTable: "set",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_card_LandscapeId",
                table: "card",
                column: "LandscapeId");

            migrationBuilder.CreateIndex(
                name: "IX_card_TypeId",
                table: "card",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cardImage_CardId",
                table: "cardImage",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_cardImage_CardImageTypeId",
                table: "cardImage",
                column: "CardImageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cardSet_CardsId",
                table: "cardSet",
                column: "CardsId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cardImage");

            migrationBuilder.DropTable(
                name: "cardSet");

            migrationBuilder.DropTable(
                name: "darklands_cardimage");

            migrationBuilder.DropTable(
                name: "card");

            migrationBuilder.DropTable(
                name: "set");

            migrationBuilder.DropTable(
                name: "cardImageType");

            migrationBuilder.DropTable(
                name: "darklands_card");

            migrationBuilder.DropTable(
                name: "cardType");

            migrationBuilder.DropTable(
                name: "landscape");

            migrationBuilder.DropTable(
                name: "darklands_cardtype");

            migrationBuilder.DropTable(
                name: "darklands_overlord");
        }
    }
}
