using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Configure_Relationship_For_Lessons_Register : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TotalPoint",
                table: "AppLessonsRegister",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "AppLessonsRegister",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppLessonsRegister_ClassId",
                table: "AppLessonsRegister",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppLessonsRegister_AppClass_ClassId",
                table: "AppLessonsRegister",
                column: "ClassId",
                principalTable: "AppClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppLessonsRegister_AppClass_ClassId",
                table: "AppLessonsRegister");

            migrationBuilder.DropIndex(
                name: "IX_AppLessonsRegister_ClassId",
                table: "AppLessonsRegister");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "AppLessonsRegister");

            migrationBuilder.AlterColumn<float>(
                name: "TotalPoint",
                table: "AppLessonsRegister",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
