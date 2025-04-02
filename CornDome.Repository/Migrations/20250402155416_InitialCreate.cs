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
                name: "card",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_card", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cardImageType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descriptor = table.Column<string>(type: "TEXT", nullable: false)
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
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "landscape",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
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
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_set", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "revision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RevisionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ability = table.Column<string>(type: "TEXT", nullable: true),
                    SetId = table.Column<int>(type: "INTEGER", nullable: true),
                    LandscapeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Cost = table.Column<int>(type: "INTEGER", nullable: true),
                    Attack = table.Column<int>(type: "INTEGER", nullable: true),
                    Defense = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_revision_cardType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "cardType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_revision_set_SetId",
                        column: x => x.SetId,
                        principalTable: "set",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "cardImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RevisionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CardImageTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false)
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
                        name: "FK_cardImage_revision_RevisionId",
                        column: x => x.RevisionId,
                        principalTable: "revision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cardImage_CardImageTypeId",
                table: "cardImage",
                column: "CardImageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cardImage_RevisionId",
                table: "cardImage",
                column: "RevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_revision_CardId",
                table: "revision",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_revision_LandscapeId",
                table: "revision",
                column: "LandscapeId");

            migrationBuilder.CreateIndex(
                name: "IX_revision_SetId",
                table: "revision",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_revision_TypeId",
                table: "revision",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cardImage");

            migrationBuilder.DropTable(
                name: "cardImageType");

            migrationBuilder.DropTable(
                name: "revision");

            migrationBuilder.DropTable(
                name: "cardType");

            migrationBuilder.DropTable(
                name: "card");

            migrationBuilder.DropTable(
                name: "landscape");

            migrationBuilder.DropTable(
                name: "set");
        }
    }
}
