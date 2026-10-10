using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main
{
    /// <inheritdoc />
    public partial class InsertFlags : Migration
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
                        "EnableMaintenanceMessage",
                        "Enables the maintenance message",
                        false
                    },
                    {
                        "EnableTournaments",
                        "Enables the tournament feature",
                        false
                    },
                    {
                        "EnableDeckComments",
                        "Allow users to upvote and leave comments.",
                        false
                    },
                    {
                        "EnableDeckAnalytics",
                        "Shows deck analysis to the user",
                        false
                    },
                    {
                        "EnableDeveloperApi",
                        "Enable experimental application features.",
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
                    'EnableMaintenanceMessage',
                    'EnableTournaments',
                    'EnableDeckComments',
                    'EnableDeckAnalytics',
                    'EnableDeveloperApi'
                );
                """);
        }
    }
}
