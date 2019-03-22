using Microsoft.EntityFrameworkCore.Migrations;

namespace BuildTrackerApi.Migrations
{
    public partial class ModifyRoleValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                nullable: true,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 0);
        }
    }
}
