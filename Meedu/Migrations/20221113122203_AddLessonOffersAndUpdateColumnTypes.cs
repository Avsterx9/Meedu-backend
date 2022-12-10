using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meedu.Migrations
{
    public partial class AddLessonOffersAndUpdateColumnTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTimespans_Users_ReservedById",
                table: "ScheduleTimespans");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleTimespans_ReservedById",
                table: "ScheduleTimespans");

            migrationBuilder.DropColumn(
                name: "ReservationDate",
                table: "ScheduleTimespans");

            migrationBuilder.DropColumn(
                name: "ReservedById",
                table: "ScheduleTimespans");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AvailableTo",
                table: "ScheduleTimespans",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AvailableFrom",
                table: "ScheduleTimespans",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.CreateTable(
                name: "LessonReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduleTimespanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonReservations_ScheduleTimespans_ScheduleTimespanId",
                        column: x => x.ScheduleTimespanId,
                        principalTable: "ScheduleTimespans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonReservations_Users_ReservedById",
                        column: x => x.ReservedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonReservations_ReservedById",
                table: "LessonReservations",
                column: "ReservedById");

            migrationBuilder.CreateIndex(
                name: "IX_LessonReservations_ScheduleTimespanId",
                table: "LessonReservations",
                column: "ScheduleTimespanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonReservations");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AvailableTo",
                table: "ScheduleTimespans",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AvailableFrom",
                table: "ScheduleTimespans",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationDate",
                table: "ScheduleTimespans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ReservedById",
                table: "ScheduleTimespans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTimespans_ReservedById",
                table: "ScheduleTimespans",
                column: "ReservedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTimespans_Users_ReservedById",
                table: "ScheduleTimespans",
                column: "ReservedById",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
