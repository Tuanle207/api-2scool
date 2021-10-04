using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Add_Status_To_LessonRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AppLessonsRegister",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppLessonsRegister");
        }
    }
}
