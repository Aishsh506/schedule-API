using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddChecksForLessonProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SingularLessons");

            migrationBuilder.DropTable(
                name: "WeeklyLessons");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddCheckConstraint(
                name: "DayOfWeek",
                table: "Lessons",
                sql: "DayOfWeek > 0 AND DayOfWeek < 6");

            migrationBuilder.AddCheckConstraint(
                name: "StartDate",
                table: "Lessons",
                sql: "StartDate <= EndDate");

            migrationBuilder.AddCheckConstraint(
                name: "Timeslot",
                table: "Lessons",
                sql: "Timeslot > 0 AND Timeslot < 8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "DayOfWeek",
                table: "Lessons");

            migrationBuilder.DropCheckConstraint(
                name: "StartDate",
                table: "Lessons");

            migrationBuilder.DropCheckConstraint(
                name: "Timeslot",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Lessons");

            migrationBuilder.CreateTable(
                name: "SingularLessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingularLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SingularLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyLessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SingularLessons_LessonId",
                table: "SingularLessons",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyLessons_LessonId",
                table: "WeeklyLessons",
                column: "LessonId");
        }
    }
}
