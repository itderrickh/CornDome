using CornDome.Models.Cards;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    /// <inheritdoc />
    public partial class DarklandsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "darklands_cardtype",
                columns: new[] { "Id", "Value", "Description" },
                values: new object[,]
                {
                    { 0, nameof(DarklandsCardTypeEnum.Creature), DarklandsCardTypeConverter.ToString(DarklandsCardTypeEnum.Creature) },
                    { 1, nameof(DarklandsCardTypeEnum.Overlord), DarklandsCardTypeConverter.ToString(DarklandsCardTypeEnum.Overlord) },
                    { 2, nameof(DarklandsCardTypeEnum.Curse), DarklandsCardTypeConverter.ToString(DarklandsCardTypeEnum.Curse) },
                    { 3, nameof(DarklandsCardTypeEnum.Barrier), DarklandsCardTypeConverter.ToString(DarklandsCardTypeEnum.Barrier) },
                    { 4, nameof(DarklandsCardTypeEnum.DifficultySetting), DarklandsCardTypeConverter.ToString(DarklandsCardTypeEnum.DifficultySetting) }
                });

            migrationBuilder.InsertData(
                table: "darklands_overlord",
                columns: new[] { "Id", "Value", "Description" },
                values: new object[,]
                {
                    { 0, nameof(DarklandsOverlordEnum.None), DarklandsOverlordEnumConverter.ToString(DarklandsOverlordEnum.None) },
                    { 1, nameof(DarklandsOverlordEnum.CaptainRatbeard), DarklandsOverlordEnumConverter.ToString(DarklandsOverlordEnum.CaptainRatbeard) },
                    { 2, nameof(DarklandsOverlordEnum.SkyklorTheOozemother), DarklandsOverlordEnumConverter.ToString(DarklandsOverlordEnum.SkyklorTheOozemother) },
                    { 3, nameof(DarklandsOverlordEnum.QuarksExperiment), DarklandsOverlordEnumConverter.ToString(DarklandsOverlordEnum.QuarksExperiment) },
                    { 4, nameof(DarklandsOverlordEnum.TheCrushasaurus), DarklandsOverlordEnumConverter.ToString(DarklandsOverlordEnum.TheCrushasaurus) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "darklands_overlord",
                keyColumn: "Id",
                keyValues: new object[] { 0, 1, 2, 3, 4 });

            migrationBuilder.DeleteData(
                table: "darklands_cardtype",
                keyColumn: "Id",
                keyValues: new object[] { 0, 1, 2, 3, 4 });
        }
    }
}
