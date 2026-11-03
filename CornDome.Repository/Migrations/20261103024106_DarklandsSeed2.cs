using CornDome.Models.Cards;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    /// <inheritdoc />
    public partial class DarklandsSeed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "darklands_overlord",
                columns: new[] { "Id", "Value", "Description" },
                values: new object[,]
                {
                    { 5, nameof(DarklandsOverlordEnum.TheThreeWeeFates), DarklandsOverlordEnumConverter.ToString(DarklandsOverlordEnum.TheThreeWeeFates) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "darklands_overlord",
                keyColumn: "Id",
                keyValues: new object[] { 5 });
        }
    }
}
