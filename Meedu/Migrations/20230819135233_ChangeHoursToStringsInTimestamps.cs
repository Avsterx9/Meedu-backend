using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meedu.Migrations;

public partial class ChangeHoursToStringsInTimestamps : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "AvailableTo",
            table: "ScheduleTimespans",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime2");

        migrationBuilder.AlterColumn<string>(
            name: "AvailableFrom",
            table: "ScheduleTimespans",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime2");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "AvailableTo",
            table: "ScheduleTimespans",
            type: "datetime2",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<DateTime>(
            name: "AvailableFrom",
            table: "ScheduleTimespans",
            type: "datetime2",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }
}
