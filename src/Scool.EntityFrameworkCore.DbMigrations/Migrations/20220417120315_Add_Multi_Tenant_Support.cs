using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Add_Multi_Tenant_Support : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppClass_AppTeacher_FormTeacherId",
                table: "AppClass");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAssignment_AppAccount_AssigneeId",
                table: "AppTaskAssignment");

            migrationBuilder.DropIndex(
                name: "IX_AppClass_FormTeacherId",
                table: "AppClass");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppTeacher",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppTeacher",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssigneeId",
                table: "AppTaskAssignment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppTaskAssignment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppStudent",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppStudent",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppRegulation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppLessonsRegister",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppLessonRegisterPhotos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppGrade",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppDcpStudentReport",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppDcpReport",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppDcpClassReportItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppDcpClassReport",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCriteria",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppCourse",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCourse",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FormTeacherId",
                table: "AppClass",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppClass",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppClass",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppActivityParticipant",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppActivity",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppActivity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppAccount",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppClass_FormTeacherId",
                table: "AppClass",
                column: "FormTeacherId",
                unique: true,
                filter: "[FormTeacherId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AppClass_AppTeacher_FormTeacherId",
                table: "AppClass",
                column: "FormTeacherId",
                principalTable: "AppTeacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAssignment_AppAccount_AssigneeId",
                table: "AppTaskAssignment",
                column: "AssigneeId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppClass_AppTeacher_FormTeacherId",
                table: "AppClass");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAssignment_AppAccount_AssigneeId",
                table: "AppTaskAssignment");

            migrationBuilder.DropIndex(
                name: "IX_AppClass_FormTeacherId",
                table: "AppClass");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppTeacher");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppTeacher");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppTaskAssignment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppStudent");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppStudent");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppRegulation");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppLessonsRegister");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppLessonRegisterPhotos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppGrade");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppDcpStudentReport");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppDcpReport");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppDcpClassReportItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppDcpClassReport");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCriteria");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppCourse");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCourse");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppClass");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppClass");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppActivityParticipant");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppActivity");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppActivity");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppAccount");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssigneeId",
                table: "AppTaskAssignment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FormTeacherId",
                table: "AppClass",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppClass_FormTeacherId",
                table: "AppClass",
                column: "FormTeacherId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppClass_AppTeacher_FormTeacherId",
                table: "AppClass",
                column: "FormTeacherId",
                principalTable: "AppTeacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAssignment_AppAccount_AssigneeId",
                table: "AppTaskAssignment",
                column: "AssigneeId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
