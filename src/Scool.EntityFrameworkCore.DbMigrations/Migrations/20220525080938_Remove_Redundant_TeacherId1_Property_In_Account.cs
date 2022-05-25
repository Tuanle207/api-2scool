using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Remove_Redundant_TeacherId1_Property_In_Account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAccount_AppTeacher_TeacherId1",
                table: "AppAccount");

            migrationBuilder.DropIndex(
                name: "IX_AppAccount_TeacherId1",
                table: "AppAccount");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "AppAccount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId1",
                table: "AppAccount",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppAccount_TeacherId1",
                table: "AppAccount",
                column: "TeacherId1",
                unique: true,
                filter: "[TeacherId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAccount_AppTeacher_TeacherId1",
                table: "AppAccount",
                column: "TeacherId1",
                principalTable: "AppTeacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
