using CornDome.Models.Cards;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    /// <inheritdoc />
    public partial class DarklandsSeed3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "darklands_cardtype",
                columns: new[] { "Id", "Value", "Description" },
                values: new object[,]
                {
                    { 5, nameof(DarklandsCardTypeEnum.Landscape), DarklandsCardTypeConverter.ToString(DarklandsCardTypeEnum.Landscape) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "darklands_cardtype",
                keyColumn: "Id",
                keyValues: new object[] { 5 });
        }
    }
}
