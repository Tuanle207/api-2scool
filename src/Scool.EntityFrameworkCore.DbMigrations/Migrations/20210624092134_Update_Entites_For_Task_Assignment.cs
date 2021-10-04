using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Update_Entites_For_Task_Assignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAssignment_AppClass_ClassId",
                table: "AppTaskAssignment");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "AppTaskAssignment",
                newName: "ClassAssignedId");

            migrationBuilder.RenameIndex(
                name: "IX_AppTaskAssignment_ClassId",
                table: "AppTaskAssignment",
                newName: "IX_AppTaskAssignment_ClassAssignedId");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "AppUserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskType",
                table: "AppTaskAssignment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppLessonRegisterPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonRegisterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLessonRegisterPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLessonRegisterPhotos_AppLessonsRegister_LessonRegisterId",
                        column: x => x.LessonRegisterId,
                        principalTable: "AppLessonsRegister",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserProfile_ClassId",
                table: "AppUserProfile",
                column: "ClassId",
                unique: true,
                filter: "[ClassId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppLessonRegisterPhotos_LessonRegisterId",
                table: "AppLessonRegisterPhotos",
                column: "LessonRegisterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAssignment_AppClass_ClassAssignedId",
                table: "AppTaskAssignment",
                column: "ClassAssignedId",
                principalTable: "AppClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserProfile_AppClass_ClassId",
                table: "AppUserProfile",
                column: "ClassId",
                principalTable: "AppClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskAssignment_AppClass_ClassAssignedId",
                table: "AppTaskAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserProfile_AppClass_ClassId",
                table: "AppUserProfile");

            migrationBuilder.DropTable(
                name: "AppLessonRegisterPhotos");

            migrationBuilder.DropIndex(
                name: "IX_AppUserProfile_ClassId",
                table: "AppUserProfile");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "AppUserProfile");

            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "AppTaskAssignment");

            migrationBuilder.RenameColumn(
                name: "ClassAssignedId",
                table: "AppTaskAssignment",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_AppTaskAssignment_ClassAssignedId",
                table: "AppTaskAssignment",
                newName: "IX_AppTaskAssignment_ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskAssignment_AppClass_ClassId",
                table: "AppTaskAssignment",
                column: "ClassId",
                principalTable: "AppClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
