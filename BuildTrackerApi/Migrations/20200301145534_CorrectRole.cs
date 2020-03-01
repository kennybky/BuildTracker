using Microsoft.EntityFrameworkCore.Migrations;

namespace BuildTrackerApi.Migrations
{
    public partial class CorrectRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1000);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1003);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1000, "973b061e-310b-4ccd-b5e7-0bca9563de40", "USER", null },
                    { 1001, "2844a0c7-47f5-46a1-8670-1ff299e50d02", "DEVELOPER", null },
                    { 1002, "7eb60ba2-0bd6-4b5d-86aa-b2aefb0005dc", "PROJECT_MANAGER", null },
                    { 1003, "6b86aec4-bd67-4fb2-aef2-81b501608a63", "ADMIN", null }
                });
        }
    }
}
