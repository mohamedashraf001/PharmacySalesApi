using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class d : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "username",
                table: "Operations",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Customers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Customers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Customers",
                newName: "CustomerMoney");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Operations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CustomerId",
                table: "Operations",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Customers_CustomerId",
                table: "Operations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Customers_CustomerId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_CustomerId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Operations");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Operations",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Customers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Customers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CustomerMoney",
                table: "Customers",
                newName: "CustomerId");
        }
    }
}
