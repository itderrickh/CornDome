using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Tournament
{
    /// <inheritdoc />
    public partial class OpenListFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOpenList",
                table: "tournament",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOpenList",
                table: "tournament");
        }
    }
}
