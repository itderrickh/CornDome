using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations.Main;

/// <inheritdoc />
public partial class _20261119012624_AddPatreonConnection : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PatreonConnection",
            columns: table => new
            {
                UserId = table.Column<int>(type: "INTEGER", nullable: false),
                PatreonUserId = table.Column<string>(type: "TEXT", nullable: false),
                PatreonMemberId = table.Column<string>(type: "TEXT", nullable: true),
                CurrentTierId = table.Column<string>(type: "TEXT", nullable: true),
                AccessToken = table.Column<string>(type: "TEXT", nullable: true),
                RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                TokenExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PatreonConnection", x => x.UserId);
                table.ForeignKey(
                    name: "FK_PatreonConnection_User_UserId",
                    column: x => x.UserId,
                    principalTable: "User",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PatreonConnection_PatreonUserId",
            table: "PatreonConnection",
            column: "PatreonUserId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PatreonConnection");
    }
}
