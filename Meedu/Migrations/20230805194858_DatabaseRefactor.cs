using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meedu.Migrations;

public partial class DatabaseRefactor : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_DaySchedules_PrivateLessonOffers_PrivateLessonOfferId",
            table: "DaySchedules");

        migrationBuilder.DropForeignKey(
            name: "FK_DaySchedules_Subjects_SubjectId",
            table: "DaySchedules");

        migrationBuilder.DropIndex(
            name: "IX_DaySchedules_PrivateLessonOfferId",
            table: "DaySchedules");

        migrationBuilder.DropIndex(
            name: "IX_DaySchedules_SubjectId",
            table: "DaySchedules");

        migrationBuilder.DropColumn(
            name: "PrivateLessonOfferId",
            table: "DaySchedules");

        migrationBuilder.DropColumn(
            name: "SubjectId",
            table: "DaySchedules");

        migrationBuilder.RenameColumn(
            name: "OnlineLessonsPossible",
            table: "PrivateLessonOffers",
            newName: "IsRemote");

        migrationBuilder.AlterColumn<int>(
            name: "TeachingRange",
            table: "PrivateLessonOffers",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "PrivateLessonOffers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "IsRemote",
            table: "PrivateLessonOffers",
            newName: "OnlineLessonsPossible");

        migrationBuilder.AlterColumn<int>(
            name: "TeachingRange",
            table: "PrivateLessonOffers",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "PrivateLessonOffers",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AddColumn<Guid>(
            name: "PrivateLessonOfferId",
            table: "DaySchedules",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "SubjectId",
            table: "DaySchedules",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex(
            name: "IX_DaySchedules_PrivateLessonOfferId",
            table: "DaySchedules",
            column: "PrivateLessonOfferId");

        migrationBuilder.CreateIndex(
            name: "IX_DaySchedules_SubjectId",
            table: "DaySchedules",
            column: "SubjectId");

        migrationBuilder.AddForeignKey(
            name: "FK_DaySchedules_PrivateLessonOffers_PrivateLessonOfferId",
            table: "DaySchedules",
            column: "PrivateLessonOfferId",
            principalTable: "PrivateLessonOffers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_DaySchedules_Subjects_SubjectId",
            table: "DaySchedules",
            column: "SubjectId",
            principalTable: "Subjects",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
