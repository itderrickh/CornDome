using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main;

/// <inheritdoc />
public partial class _20261119165956_EarlyBackerRole : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Role",
            columns: ["Name"],
            values: new object[,]
            {
                    {
                        "Early_Backer",
                    }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
                DELETE FROM AppFlags
                WHERE Name IN (
                    'Early_Backer'
                );
                """);
    }
}
