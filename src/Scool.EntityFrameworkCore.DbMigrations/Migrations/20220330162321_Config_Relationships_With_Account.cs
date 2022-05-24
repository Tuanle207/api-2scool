using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Config_Relationships_With_Account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "AppTaskAssignment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppTaskAssignment",
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
                name: "LastUpdationTime",
                table: "AppActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatorId",
                table: "AppActivity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId1",
                table: "AppAccount",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId1",
                table: "AppAccount",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskAssignment_AssigneeId",
                table: "AppTaskAssignment",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskAssignment_CreatorId",
                table: "AppTaskAssignment",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRegulation_CreatorId",
                table: "AppRegulation",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLessonsRegister_CreatorId",
                table: "AppLessonsRegister",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDcpReport_CreatorId",
                table: "AppDcpReport",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCriteria_CreatorId",
                table: "AppCriteria",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppActivity_CreatorId",
                table: "AppActivity",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppActivity_LastUpdatorId",
                table: "AppActivity",
                column: "LastUpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAccount_ClassId",
                table: "AppAccount",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAccount_StudentId",
                table: "AppAccount",
                column: "StudentId",
                unique: true,
                filter: "[StudentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppAccount_StudentId1",
                table: "AppAccount",
                column: "StudentId1",
                unique: true,
                filter: "[StudentId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppAccount_TeacherId",
                table: "AppAccount",
                column: "TeacherId",
                unique: true,
                filter: "[TeacherId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppAccount_TeacherId1",
                table: "AppAccount",
                column: "TeacherId1",
                unique: true,
                filter: "[TeacherId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAccount_AppClass_ClassId",
                table: "AppAccount",
                column: "ClassId",
                principalTable: "AppClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppAccount_AppStudent_StudentId",
                table: "AppAccount",
                column: "StudentId",
                principalTable: "AppStudent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppAccount_AppStudent_StudentId1",
                table: "AppAccount",
                column: "StudentId1",
                principalTable: "AppStudent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppAccount_AppTeacher_TeacherId",
                table: "AppAccount",
                column: "TeacherId",
                principalTable: "AppTeacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppAccount_AppTeacher_TeacherId1",
                table: "AppAccount",
                column: "TeacherId1",
                principalTable: "AppTeacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppActivity_AppAccount_CreatorId",
                table: "AppActivity",
                column: "CreatorId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppActivity_AppAccount_LastUpdatorId",
                table: "AppActivity",
                column: "LastUpdatorId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCriteria_AppAccount_CreatorId",
                table: "AppCriteria",
                column: "CreatorId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppDcpReport_AppAccount_CreatorId",
                table: "AppDcpReport",
                column: "CreatorId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppLessonsRegister_AppAccount_CreatorId",
                table: "AppLessonsRegister",
                column: "CreatorId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegulation_AppAccount_CreatorId",
                table: "AppRegulation",
                column: "CreatorId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAssignment_AppAccount_AssigneeId",
                table: "AppTaskAssignment",
                column: "AssigneeId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAssignment_AppAccount_CreatorId",
                table: "AppTaskAssignment",
                column: "CreatorId",
                principalTable: "AppAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAccount_AppClass_ClassId",
                table: "AppAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_AppAccount_AppStudent_StudentId",
                table: "AppAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_AppAccount_AppStudent_StudentId1",
                table: "AppAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_AppAccount_AppTeacher_TeacherId",
                table: "AppAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_AppAccount_AppTeacher_TeacherId1",
                table: "AppAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_AppActivity_AppAccount_CreatorId",
                table: "AppActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_AppActivity_AppAccount_LastUpdatorId",
                table: "AppActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCriteria_AppAccount_CreatorId",
                table: "AppCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDcpReport_AppAccount_CreatorId",
                table: "AppDcpReport");

            migrationBuilder.DropForeignKey(
                name: "FK_AppLessonsRegister_AppAccount_CreatorId",
                table: "AppLessonsRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRegulation_AppAccount_CreatorId",
                table: "AppRegulation");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAssignment_AppAccount_AssigneeId",
                table: "AppTaskAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAssignment_AppAccount_CreatorId",
                table: "AppTaskAssignment");

            migrationBuilder.DropIndex(
                name: "IX_AppTaskAssignment_AssigneeId",
                table: "AppTaskAssignment");

            migrationBuilder.DropIndex(
                name: "IX_AppTaskAssignment_CreatorId",
                table: "AppTaskAssignment");

            migrationBuilder.DropIndex(
                name: "IX_AppRegulation_CreatorId",
                table: "AppRegulation");

            migrationBuilder.DropIndex(
                name: "IX_AppLessonsRegister_CreatorId",
                table: "AppLessonsRegister");

            migrationBuilder.DropIndex(
                name: "IX_AppDcpReport_CreatorId",
                table: "AppDcpReport");

            migrationBuilder.DropIndex(
                name: "IX_AppCriteria_CreatorId",
                table: "AppCriteria");

            migrationBuilder.DropIndex(
                name: "IX_AppActivity_CreatorId",
                table: "AppActivity");

            migrationBuilder.DropIndex(
                name: "IX_AppActivity_LastUpdatorId",
                table: "AppActivity");

            migrationBuilder.DropIndex(
                name: "IX_AppAccount_ClassId",
                table: "AppAccount");

            migrationBuilder.DropIndex(
                name: "IX_AppAccount_StudentId",
                table: "AppAccount");

            migrationBuilder.DropIndex(
                name: "IX_AppAccount_StudentId1",
                table: "AppAccount");

            migrationBuilder.DropIndex(
                name: "IX_AppAccount_TeacherId",
                table: "AppAccount");

            migrationBuilder.DropIndex(
                name: "IX_AppAccount_TeacherId1",
                table: "AppAccount");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "AppTaskAssignment");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppTaskAssignment");

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
                name: "LastUpdationTime",
                table: "AppActivity");

            migrationBuilder.DropColumn(
                name: "LastUpdatorId",
                table: "AppActivity");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "AppAccount");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "AppAccount");
        }
    }
}
