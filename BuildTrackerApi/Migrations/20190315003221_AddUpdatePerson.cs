using Microsoft.EntityFrameworkCore.Migrations;

namespace BuildTrackerApi.Migrations
{
    public partial class AddUpdatePerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Users_BuildPersonId",
                table: "Builds");

            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Products_ProductName",
                table: "Builds");

            migrationBuilder.AddColumn<int>(
                name: "UpdatePersonId",
                table: "Builds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Builds_UpdatePersonId",
                table: "Builds",
                column: "UpdatePersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Users_BuildPersonId",
                table: "Builds",
                column: "BuildPersonId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Products_ProductName",
                table: "Builds",
                column: "ProductName",
                principalTable: "Products",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Users_UpdatePersonId",
                table: "Builds",
                column: "UpdatePersonId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Users_BuildPersonId",
                table: "Builds");

            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Products_ProductName",
                table: "Builds");

            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Users_UpdatePersonId",
                table: "Builds");

            migrationBuilder.DropIndex(
                name: "IX_Builds_UpdatePersonId",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "UpdatePersonId",
                table: "Builds");

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Users_BuildPersonId",
                table: "Builds",
                column: "BuildPersonId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Products_ProductName",
                table: "Builds",
                column: "ProductName",
                principalTable: "Products",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
