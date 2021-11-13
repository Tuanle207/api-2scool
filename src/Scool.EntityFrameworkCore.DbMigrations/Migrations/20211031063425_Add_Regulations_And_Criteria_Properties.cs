using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Add_Regulations_And_Criteria_Properties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppRegulation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppRegulation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AppRegulation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppRegulation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AppRegulation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppCriteria",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppCriteria",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppCriteria",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppRegulation");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppRegulation");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AppRegulation");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppRegulation");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppRegulation");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppCriteria");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppCriteria");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppCriteria");
        }
    }
}
