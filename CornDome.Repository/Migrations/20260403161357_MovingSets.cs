using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    /// <inheritdoc />
    public partial class MovingSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_revision_set_SetId",
                table: "revision");

            migrationBuilder.DropIndex(
                name: "IX_revision_SetId",
                table: "revision");

            migrationBuilder.DropColumn(
                name: "SetId",
                table: "revision");

            migrationBuilder.CreateTable(
                name: "revisionSet",
                columns: table => new
                {
                    CardSetsId = table.Column<int>(type: "INTEGER", nullable: false),
                    RevisionsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revisionSet", x => new { x.CardSetsId, x.RevisionsId });
                    table.ForeignKey(
                        name: "FK_revisionSet_revision_RevisionsId",
                        column: x => x.RevisionsId,
                        principalTable: "revision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_revisionSet_set_CardSetsId",
                        column: x => x.CardSetsId,
                        principalTable: "set",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_revisionSet_RevisionsId",
                table: "revisionSet",
                column: "RevisionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "revisionSet");

            migrationBuilder.AddColumn<int>(
                name: "SetId",
                table: "revision",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_revision_SetId",
                table: "revision",
                column: "SetId");

            migrationBuilder.AddForeignKey(
                name: "FK_revision_set_SetId",
                table: "revision",
                column: "SetId",
                principalTable: "set",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
