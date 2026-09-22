using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Users_UserId",
                table: "Actions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Workers");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Actions",
                newName: "ActionDate");

            migrationBuilder.RenameColumn(
                name: "QuantityChanged",
                table: "Actions",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Workers",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Workers",
                newName: "Name");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Items",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Actions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workers",
                table: "Workers",
                column: "ID");

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Email", "Name" },
                values: new object[] { "arotar2005@gmail.com", "Admin User" });

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Workers_UserId",
                table: "Actions",
                column: "UserId",
                principalTable: "Workers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Workers_UserId",
                table: "Actions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Workers",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Actions");

            migrationBuilder.RenameTable(
                name: "Workers",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Actions",
                newName: "QuantityChanged");

            migrationBuilder.RenameColumn(
                name: "ActionDate",
                table: "Actions",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "Username");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Items",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Role", "Username" },
                values: new object[] { "admin@warehouse.com", "User", "admin" });

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Users_UserId",
                table: "Actions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
