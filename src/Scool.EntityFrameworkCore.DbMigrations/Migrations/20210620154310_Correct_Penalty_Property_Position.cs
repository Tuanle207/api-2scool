using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Correct_Penalty_Property_Position : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PenaltyTotal",
                table: "AppDcpReport");

            migrationBuilder.AddColumn<int>(
                name: "PenaltyTotal",
                table: "AppDcpClassReport",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PenaltyTotal",
                table: "AppDcpClassReport");

            migrationBuilder.AddColumn<int>(
                name: "PenaltyTotal",
                table: "AppDcpReport",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
