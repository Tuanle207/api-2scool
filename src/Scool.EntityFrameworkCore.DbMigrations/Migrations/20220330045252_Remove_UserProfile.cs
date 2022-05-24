using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Remove_UserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppStudent_AppUserProfile_UserProfileId",
                table: "AppStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAssignment_AppUserProfile_AssigneeId",
                table: "AppTaskAssignment");

            migrationBuilder.DropTable(
                name: "AppUserProfile");

            migrationBuilder.DropIndex(
                name: "IX_AppTaskAssignment_AssigneeId",
                table: "AppTaskAssignment");

            migrationBuilder.DropIndex(
                name: "IX_AppStudent_UserProfileId",
                table: "AppStudent");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "AppStudent",
                newName: "AccountId");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "AppTeacher",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AppTeacher");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "AppStudent",
                newName: "UserProfileId");

            migrationBuilder.CreateTable(
                name: "AppUserProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserProfile_AppClass_ClassId",
                        column: x => x.ClassId,
                        principalTable: "AppClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskAssignment_AssigneeId",
                table: "AppTaskAssignment",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStudent_UserProfileId",
                table: "AppStudent",
                column: "UserProfileId",
                unique: true,
                filter: "[UserProfileId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserProfile_ClassId",
                table: "AppUserProfile",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppStudent_AppUserProfile_UserProfileId",
                table: "AppStudent",
                column: "UserProfileId",
                principalTable: "AppUserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAssignment_AppUserProfile_AssigneeId",
                table: "AppTaskAssignment",
                column: "AssigneeId",
                principalTable: "AppUserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
