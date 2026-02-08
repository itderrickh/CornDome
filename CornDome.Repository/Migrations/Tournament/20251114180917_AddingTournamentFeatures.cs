using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Tournament
{
    /// <inheritdoc />
    public partial class AddingTournamentFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStart",
                table: "tournament",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RoundDuration",
                table: "tournament",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "IsDQed",
                table: "registration",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDropped",
                table: "registration",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualStart",
                table: "tournament");

            migrationBuilder.DropColumn(
                name: "RoundDuration",
                table: "tournament");

            migrationBuilder.DropColumn(
                name: "IsDQed",
                table: "registration");

            migrationBuilder.DropColumn(
                name: "IsDropped",
                table: "registration");
        }
    }
}
