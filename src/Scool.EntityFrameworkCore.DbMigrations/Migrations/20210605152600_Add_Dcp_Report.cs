using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Add_Dcp_Report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRegulation_AppRegulationType_RegulationTypeId",
                table: "AppRegulation");

            migrationBuilder.DropTable(
                name: "AppRegulationType");

            migrationBuilder.DropIndex(
                name: "IX_AppRegulation_RegulationTypeId",
                table: "AppRegulation");

            migrationBuilder.DropColumn(
                name: "RegulationTypeId",
                table: "AppRegulation");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AppTaskAssignment",
                newName: "AssigneeId");

            migrationBuilder.CreateTable(
                name: "AppDcpReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PenaltyTotal = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDcpReport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppDcpClassReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DcpReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDcpClassReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDcpClassReport_AppClass_ClassId",
                        column: x => x.ClassId,
                        principalTable: "AppClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppDcpClassReport_AppDcpReport_DcpReportId",
                        column: x => x.DcpReportId,
                        principalTable: "AppDcpReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppDcpClassReportItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DcpClassReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDcpClassReportItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDcpClassReportItem_AppDcpClassReport_DcpClassReportId",
                        column: x => x.DcpClassReportId,
                        principalTable: "AppDcpClassReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppDcpClassReportItem_AppRegulation_RegulationId",
                        column: x => x.RegulationId,
                        principalTable: "AppRegulation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppDcpStudentReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DcpClassReportItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDcpStudentReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDcpStudentReport_AppDcpClassReportItem_DcpClassReportItemId",
                        column: x => x.DcpClassReportItemId,
                        principalTable: "AppDcpClassReportItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppDcpStudentReport_AppStudent_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AppStudent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDcpClassReport_ClassId",
                table: "AppDcpClassReport",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDcpClassReport_DcpReportId",
                table: "AppDcpClassReport",
                column: "DcpReportId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDcpClassReportItem_DcpClassReportId",
                table: "AppDcpClassReportItem",
                column: "DcpClassReportId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDcpClassReportItem_RegulationId",
                table: "AppDcpClassReportItem",
                column: "RegulationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDcpStudentReport_DcpClassReportItemId",
                table: "AppDcpStudentReport",
                column: "DcpClassReportItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDcpStudentReport_StudentId",
                table: "AppDcpStudentReport",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDcpStudentReport");

            migrationBuilder.DropTable(
                name: "AppUserProfile");

            migrationBuilder.DropTable(
                name: "AppDcpClassReportItem");

            migrationBuilder.DropTable(
                name: "AppDcpClassReport");

            migrationBuilder.DropTable(
                name: "AppDcpReport");

            migrationBuilder.RenameColumn(
                name: "AssigneeId",
                table: "AppTaskAssignment",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "RegulationTypeId",
                table: "AppRegulation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AppRegulationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRegulationType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRegulation_RegulationTypeId",
                table: "AppRegulation",
                column: "RegulationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegulation_AppRegulationType_RegulationTypeId",
                table: "AppRegulation",
                column: "RegulationTypeId",
                principalTable: "AppRegulationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
