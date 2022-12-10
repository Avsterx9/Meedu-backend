using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meedu.Migrations
{
    public partial class AddLessonTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LessonTitle",
                table: "PrivateLessonOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LessonTitle",
                table: "PrivateLessonOffers");
        }
    }
}
