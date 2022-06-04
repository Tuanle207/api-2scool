using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Add_Course_Id_To_Reports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "AppLessonsRegister",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "AppDcpReport",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ReportedClassDisplayNames",
                table: "AppDcpReport",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PenaltyPoint",
                table: "AppDcpClassReportItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AppLessonsRegister_CourseId",
                table: "AppLessonsRegister",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDcpReport_CourseId",
                table: "AppDcpReport",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDcpReport_AppCourse_CourseId",
                table: "AppDcpReport",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppLessonsRegister_AppCourse_CourseId",
                table: "AppLessonsRegister",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDcpReport_AppCourse_CourseId",
                table: "AppDcpReport");

            migrationBuilder.DropForeignKey(
                name: "FK_AppLessonsRegister_AppCourse_CourseId",
                table: "AppLessonsRegister");

            migrationBuilder.DropIndex(
                name: "IX_AppLessonsRegister_CourseId",
                table: "AppLessonsRegister");

            migrationBuilder.DropIndex(
                name: "IX_AppDcpReport_CourseId",
                table: "AppDcpReport");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AppLessonsRegister");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AppDcpReport");

            migrationBuilder.DropColumn(
                name: "ReportedClassDisplayNames",
                table: "AppDcpReport");

            migrationBuilder.DropColumn(
                name: "PenaltyPoint",
                table: "AppDcpClassReportItem");
        }
    }
}
