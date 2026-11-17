using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main;

/// <inheritdoc />
public partial class _20261117211331_AddingDweebleFlag : Migration
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
                        "EnableDweeble",
                        "Enables Dweeble Game",
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
                    'EnableDweeble'
                );
                """);
    }
}
