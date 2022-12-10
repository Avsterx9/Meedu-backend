using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meedu.Migrations
{
    public partial class AddPhoneNumberAndLessonOfferToSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PrivateLessonOfferId",
                table: "DaySchedules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DaySchedules_PrivateLessonOfferId",
                table: "DaySchedules",
                column: "PrivateLessonOfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_DaySchedules_PrivateLessonOffers_PrivateLessonOfferId",
                table: "DaySchedules",
                column: "PrivateLessonOfferId",
                principalTable: "PrivateLessonOffers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DaySchedules_PrivateLessonOffers_PrivateLessonOfferId",
                table: "DaySchedules");

            migrationBuilder.DropIndex(
                name: "IX_DaySchedules_PrivateLessonOfferId",
                table: "DaySchedules");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrivateLessonOfferId",
                table: "DaySchedules");
        }
    }
}
