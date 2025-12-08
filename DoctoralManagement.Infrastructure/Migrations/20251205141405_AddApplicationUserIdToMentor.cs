using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctoralManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserIdToMentor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Mentors",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Mentors");
        }
    }
}
