using Microsoft.EntityFrameworkCore.Migrations;

namespace BuildTrackerApi.Migrations
{
    public partial class ModifyRoleRequirement2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                nullable: true,
                defaultValue: 3,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 3);
        }
    }
}
