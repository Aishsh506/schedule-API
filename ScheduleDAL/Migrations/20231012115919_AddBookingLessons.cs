using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audiences_Buildings_BuildingId",
                table: "Audiences");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropCheckConstraint(
                name: "Timeslot",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Audiences_BuildingId",
                table: "Audiences");

            migrationBuilder.DropIndex(
                name: "IX_Audiences_Name_BuildingId",
                table: "Audiences");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Audiences");

            migrationBuilder.CreateTable(
                name: "BookedLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timeslot = table.Column<long>(type: "bigint", nullable: false),
                    AudienceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedLessons", x => x.Id);
                    table.CheckConstraint("Timeslot", "Timeslot > 0 AND Timeslot < 8");
                    table.ForeignKey(
                        name: "FK_BookedLessons_Audiences_AudienceId",
                        column: x => x.AudienceId,
                        principalTable: "Audiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookedLessonGroups",
                columns: table => new
                {
                    BookedLessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedLessonGroups", x => new { x.BookedLessonId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_BookedLessonGroups_BookedLessons_BookedLessonId",
                        column: x => x.BookedLessonId,
                        principalTable: "BookedLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookedLessonGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "Timeslot1",
                table: "Lessons",
                sql: "Timeslot > 0 AND Timeslot < 8");

            migrationBuilder.CreateIndex(
                name: "IX_Audiences_Name",
                table: "Audiences",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookedLessonGroups_GroupId",
                table: "BookedLessonGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BookedLessons_AudienceId",
                table: "BookedLessons",
                column: "AudienceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookedLessonGroups");

            migrationBuilder.DropTable(
                name: "BookedLessons");

            migrationBuilder.DropCheckConstraint(
                name: "Timeslot1",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Audiences_Name",
                table: "Audiences");

            migrationBuilder.AddColumn<Guid>(
                name: "BuildingId",
                table: "Audiences",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

            migrationBuilder.AddCheckConstraint(
                name: "Timeslot",
                table: "Lessons",
                sql: "Timeslot > 0 AND Timeslot < 8");

            migrationBuilder.CreateIndex(
                name: "IX_Audiences_BuildingId",
                table: "Audiences",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Audiences_Name_BuildingId",
                table: "Audiences",
                columns: new[] { "Name", "BuildingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Name",
                table: "Buildings",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Audiences_Buildings_BuildingId",
                table: "Audiences",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
