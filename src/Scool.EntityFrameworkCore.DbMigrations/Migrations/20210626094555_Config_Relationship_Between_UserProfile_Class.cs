using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Config_Relationship_Between_UserProfile_Class : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppUserProfile_ClassId",
                table: "AppUserProfile");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserProfile_ClassId",
                table: "AppUserProfile",
                column: "ClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppUserProfile_ClassId",
                table: "AppUserProfile");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserProfile_ClassId",
                table: "AppUserProfile",
                column: "ClassId",
                unique: true,
                filter: "[ClassId] IS NOT NULL");
        }
    }
}
