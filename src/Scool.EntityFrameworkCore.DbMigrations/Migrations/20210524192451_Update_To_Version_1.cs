using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scool.Migrations
{
    public partial class Update_To_Version_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCourse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCourse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCriteria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppGrade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppGrade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppLessonsRegister",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPoint = table.Column<float>(type: "real", nullable: false),
                    AbsenceNo = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLessonsRegister", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRegulationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRegulationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppTeacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTeacher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppActivity_AppCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "AppCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppRegulation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Point = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriteriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegulationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRegulation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRegulation_AppCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "AppCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppRegulation_AppCriteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "AppCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppRegulation_AppRegulationType_RegulationTypeId",
                        column: x => x.RegulationTypeId,
                        principalTable: "AppRegulationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppClass",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormTeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoStudents = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppClass_AppCourse_CourseId",
                        column: x => x.CourseId,
                        principalTable: "AppCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppClass_AppGrade_GradeId",
                        column: x => x.GradeId,
                        principalTable: "AppGrade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppClass_AppTeacher_FormTeacherId",
                        column: x => x.FormTeacherId,
                        principalTable: "AppTeacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppActivityParticipant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Place = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppActivityParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppActivityParticipant_AppActivity_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "AppActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppStudent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppStudent_AppClass_ClassId",
                        column: x => x.ClassId,
                        principalTable: "AppClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppTaskAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTaskAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTaskAssignment_AppClass_ClassId",
                        column: x => x.ClassId,
                        principalTable: "AppClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppActivity_CourseId",
                table: "AppActivity",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppActivityParticipant_ActivityId",
                table: "AppActivityParticipant",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_AppClass_CourseId",
                table: "AppClass",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppClass_FormTeacherId",
                table: "AppClass",
                column: "FormTeacherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppClass_GradeId",
                table: "AppClass",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRegulation_CourseId",
                table: "AppRegulation",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRegulation_CriteriaId",
                table: "AppRegulation",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRegulation_RegulationTypeId",
                table: "AppRegulation",
                column: "RegulationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStudent_ClassId",
                table: "AppStudent",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskAssignment_ClassId",
                table: "AppTaskAssignment",
                column: "ClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppActivityParticipant");

            migrationBuilder.DropTable(
                name: "AppLessonsRegister");

            migrationBuilder.DropTable(
                name: "AppRegulation");

            migrationBuilder.DropTable(
                name: "AppStudent");

            migrationBuilder.DropTable(
                name: "AppTaskAssignment");

            migrationBuilder.DropTable(
                name: "AppActivity");

            migrationBuilder.DropTable(
                name: "AppCriteria");

            migrationBuilder.DropTable(
                name: "AppRegulationType");

            migrationBuilder.DropTable(
                name: "AppClass");

            migrationBuilder.DropTable(
                name: "AppCourse");

            migrationBuilder.DropTable(
                name: "AppGrade");

            migrationBuilder.DropTable(
                name: "AppTeacher");
        }
    }
}
