using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main;

/// <inheritdoc />
public partial class _20261119014931_AddPatreonRoles : Migration
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
                        "The_Dweeb",
                    },
                    {
                        "The_Flooper",
                    },
                    {
                        "The_CoolGuy",
                    }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
                DELETE FROM AppFlags
                WHERE Name IN (
                    'The_Dweeb',
                    'The_Flooper',
                    'The_CoolGuy'
                );
                """);
    }
}
