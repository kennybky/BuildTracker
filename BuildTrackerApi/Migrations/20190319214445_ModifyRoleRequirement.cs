using Microsoft.EntityFrameworkCore.Migrations;

namespace BuildTrackerApi.Migrations
{
    public partial class ModifyRoleRequirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int));
        }
    }
}
