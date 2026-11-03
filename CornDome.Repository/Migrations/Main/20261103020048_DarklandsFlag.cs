using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main
{
    /// <inheritdoc />
    public partial class DarklandsFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppFlags",
                columns: ["Name", "Description", "Active"],
                values: new object[,]
                {
                    {
                        "EnableDarkLands",
                        "Enables the DarkLands database",
                        false
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM AppFlags
                WHERE Name IN (
                    'EnableDarkLands'
                );
                """);
        }
    }
}
