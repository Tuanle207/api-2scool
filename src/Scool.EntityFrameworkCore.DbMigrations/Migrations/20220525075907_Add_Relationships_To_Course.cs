using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Add_Relationships_To_Course : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAccount_AppStudent_StudentId1",
                table: "AppAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_AppActivity_AppCourse_CourseId",
                table: "AppActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_AppClass_AppCourse_CourseId",
                table: "AppClass");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRegulation_AppCourse_CourseId",
                table: "AppRegulation");

            migrationBuilder.DropIndex(
                name: "IX_AppAccount_StudentId1",
                table: "AppAccount");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AppGrade");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "AppAccount");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "AppTeacher",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "AppTaskAssignment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "AppStudent",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "AppRegulation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GradeCode",
                table: "AppGrade",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AppCourse",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AppTeacher_CourseId",
                table: "AppTeacher",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskAssignment_CourseId",
                table: "AppTaskAssignment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStudent_CourseId",
                table: "AppStudent",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppActivity_AppCourse_CourseId",
                table: "AppActivity",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppClass_AppCourse_CourseId",
                table: "AppClass",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegulation_AppCourse_CourseId",
                table: "AppRegulation",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppStudent_AppCourse_CourseId",
                table: "AppStudent",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAssignment_AppCourse_CourseId",
                table: "AppTaskAssignment",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTeacher_AppCourse_CourseId",
                table: "AppTeacher",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppActivity_AppCourse_CourseId",
                table: "AppActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_AppClass_AppCourse_CourseId",
                table: "AppClass");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRegulation_AppCourse_CourseId",
                table: "AppRegulation");

            migrationBuilder.DropForeignKey(
                name: "FK_AppStudent_AppCourse_CourseId",
                table: "AppStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAssignment_AppCourse_CourseId",
                table: "AppTaskAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTeacher_AppCourse_CourseId",
                table: "AppTeacher");

            migrationBuilder.DropIndex(
                name: "IX_AppTeacher_CourseId",
                table: "AppTeacher");

            migrationBuilder.DropIndex(
                name: "IX_AppTaskAssignment_CourseId",
                table: "AppTaskAssignment");

            migrationBuilder.DropIndex(
                name: "IX_AppStudent_CourseId",
                table: "AppStudent");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AppTeacher");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AppTaskAssignment");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AppStudent");

            migrationBuilder.DropColumn(
                name: "GradeCode",
                table: "AppGrade");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AppCourse");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "AppRegulation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AppGrade",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId1",
                table: "AppAccount",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppAccount_StudentId1",
                table: "AppAccount",
                column: "StudentId1",
                unique: true,
                filter: "[StudentId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAccount_AppStudent_StudentId1",
                table: "AppAccount",
                column: "StudentId1",
                principalTable: "AppStudent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppActivity_AppCourse_CourseId",
                table: "AppActivity",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppClass_AppCourse_CourseId",
                table: "AppClass",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegulation_AppCourse_CourseId",
                table: "AppRegulation",
                column: "CourseId",
                principalTable: "AppCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
