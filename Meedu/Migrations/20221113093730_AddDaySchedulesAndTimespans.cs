using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meedu.Migrations
{
    public partial class AddDaySchedulesAndTimespans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DaySchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaySchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DaySchedules_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DaySchedules_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTimespans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableFrom = table.Column<TimeSpan>(type: "time", nullable: false),
                    AvailableTo = table.Column<TimeSpan>(type: "time", nullable: false),
                    ReservedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTimespans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleTimespans_DaySchedules_DayScheduleId",
                        column: x => x.DayScheduleId,
                        principalTable: "DaySchedules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduleTimespans_Users_ReservedById",
                        column: x => x.ReservedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DaySchedules_SubjectId",
                table: "DaySchedules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DaySchedules_UserId",
                table: "DaySchedules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTimespans_DayScheduleId",
                table: "ScheduleTimespans",
                column: "DayScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTimespans_ReservedById",
                table: "ScheduleTimespans",
                column: "ReservedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleTimespans");

            migrationBuilder.DropTable(
                name: "DaySchedules");
        }
    }
}
