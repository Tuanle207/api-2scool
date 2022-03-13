using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Add_Relation_Between_UserProfile_And_Student : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "AppUserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "AppStudent",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppStudent_UserProfileId",
                table: "AppStudent",
                column: "UserProfileId",
                unique: true,
                filter: "[UserProfileId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AppStudent_AppUserProfile_UserProfileId",
                table: "AppStudent",
                column: "UserProfileId",
                principalTable: "AppUserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppStudent_AppUserProfile_UserProfileId",
                table: "AppStudent");

            migrationBuilder.DropIndex(
                name: "IX_AppStudent_UserProfileId",
                table: "AppStudent");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "AppUserProfile");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "AppStudent");
        }
    }
}
