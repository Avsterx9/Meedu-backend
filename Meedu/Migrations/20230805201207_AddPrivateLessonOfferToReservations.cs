using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meedu.Migrations;

public partial class AddPrivateLessonOfferToReservations : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "PrivateLessonOfferId",
            table: "LessonReservations",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex(
            name: "IX_LessonReservations_PrivateLessonOfferId",
            table: "LessonReservations",
            column: "PrivateLessonOfferId");

        migrationBuilder.AddForeignKey(
            name: "FK_LessonReservations_PrivateLessonOffers_PrivateLessonOfferId",
            table: "LessonReservations",
            column: "PrivateLessonOfferId",
            principalTable: "PrivateLessonOffers",
            principalColumn: "Id",
            onDelete: ReferentialAction.NoAction);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_LessonReservations_PrivateLessonOffers_PrivateLessonOfferId",
            table: "LessonReservations");

        migrationBuilder.DropIndex(
            name: "IX_LessonReservations_PrivateLessonOfferId",
            table: "LessonReservations");

        migrationBuilder.DropColumn(
            name: "PrivateLessonOfferId",
            table: "LessonReservations");
    }
}
