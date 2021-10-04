using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Config_Relationshiop_Between_UserProfile_TaskAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUserTaskAsisgnment",
                columns: table => new
                {
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserTaskAsisgnment", x => new { x.UserProfileId, x.TaskAssignmentId });
                    table.ForeignKey(
                        name: "FK_AppUserTaskAsisgnment_AppTaskAssignment_TaskAssignmentId",
                        column: x => x.TaskAssignmentId,
                        principalTable: "AppTaskAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserTaskAsisgnment_AppUserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "AppUserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserTaskAsisgnment_TaskAssignmentId",
                table: "AppUserTaskAsisgnment",
                column: "TaskAssignmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserTaskAsisgnment");
        }
    }
}
