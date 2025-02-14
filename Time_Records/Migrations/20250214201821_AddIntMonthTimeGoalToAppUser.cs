using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Time_Records.Migrations
{
    /// <inheritdoc />
    public partial class AddIntMonthTimeGoalToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthTimeGoal",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthTimeGoal",
                table: "AspNetUsers");
        }
    }
}
