using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meedu.Migrations
{
    public partial class SetReservationUserAsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTimespans_Users_ReservedById",
                table: "ScheduleTimespans");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReservedById",
                table: "ScheduleTimespans",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTimespans_Users_ReservedById",
                table: "ScheduleTimespans",
                column: "ReservedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTimespans_Users_ReservedById",
                table: "ScheduleTimespans");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReservedById",
                table: "ScheduleTimespans",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTimespans_Users_ReservedById",
                table: "ScheduleTimespans",
                column: "ReservedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
