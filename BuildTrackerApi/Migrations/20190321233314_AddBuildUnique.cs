using Microsoft.EntityFrameworkCore.Migrations;

namespace BuildTrackerApi.Migrations
{
    public partial class AddBuildUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Builds_ProductName",
                table: "Builds");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Builds",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Platform",
                table: "Builds",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Builds_ProductName_Version_Platform",
                table: "Builds",
                columns: new[] { "ProductName", "Version", "Platform" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Builds_ProductName_Version_Platform",
                table: "Builds");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Builds",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Platform",
                table: "Builds",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Builds_ProductName",
                table: "Builds",
                column: "ProductName");
        }
    }
}
