using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NBAGamesNETCoreAPI.Migrations
{
    public partial class _1stUpGameMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UpcomingGames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<string>(nullable: true),
                    GameStartDateTimeUTC = table.Column<DateTime>(nullable: false),
                    TeamATriCode = table.Column<string>(nullable: true),
                    TeamAFullName = table.Column<string>(nullable: true),
                    TeamALogoSrc = table.Column<string>(nullable: true),
                    TeamBTriCode = table.Column<string>(nullable: true),
                    TeamBFullName = table.Column<string>(nullable: true),
                    TeamBLogoSrc = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpcomingGames", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UpcomingGames");
        }
    }
}
