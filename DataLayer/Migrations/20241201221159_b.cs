using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class b : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Operations",
                newName: "Operator");

            migrationBuilder.AddColumn<string>(
                name: "OperationCustomerName",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationCustomerName",
                table: "Operations");

            migrationBuilder.RenameColumn(
                name: "Operator",
                table: "Operations",
                newName: "Username");
        }
    }
}
