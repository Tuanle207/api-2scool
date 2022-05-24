using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Remove_All_Relationships_Related_To_CreatorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AppTeacher");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "AppTaskAssignment");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppTaskAssignment");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AppStudent");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppRegulation");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppLessonsRegister");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppDcpReport");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppCriteria");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppActivity");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppActivity");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "AppAccount",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "AppAccount",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "AppAccount",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "AppAccount");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "AppAccount");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "AppAccount");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "AppTeacher",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "AppTaskAssignment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppTaskAssignment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "AppStudent",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppRegulation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppLessonsRegister",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppDcpReport",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppCriteria",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppActivity",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
