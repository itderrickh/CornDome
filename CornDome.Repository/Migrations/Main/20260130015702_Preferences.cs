using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main
{
    /// <inheritdoc />
    public partial class Preferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "PlayAvailabilities");

            migrationBuilder.CreateTable(
                name: "PlayPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeZone = table.Column<string>(type: "TEXT", nullable: true),
                    GameFormat = table.Column<string>(type: "TEXT", nullable: true),
                    Pronouns = table.Column<string>(type: "TEXT", nullable: true),
                    AddressMeAs = table.Column<string>(type: "TEXT", nullable: true),
                    Platform = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayPreferences_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayPreferences_UserId",
                table: "PlayPreferences",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayPreferences");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "PlayAvailabilities",
                type: "TEXT",
                nullable: true);
        }
    }
}
