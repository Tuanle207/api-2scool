using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class _Remove_Course_Relation_On_Regulation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRegulation_AppCourse_CourseId",
                table: "AppRegulation");

            migrationBuilder.AlterColumn<int>(
                name: "Point",
                table: "AppRegulation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "AppRegulation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegulation_AppCourse_CourseId",
                table: "AppRegulation",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRegulation_AppCourse_CourseId",
                table: "AppRegulation");

            migrationBuilder.AlterColumn<string>(
                name: "Point",
                table: "AppRegulation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "AppRegulation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegulation_AppCourse_CourseId",
                table: "AppRegulation",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
