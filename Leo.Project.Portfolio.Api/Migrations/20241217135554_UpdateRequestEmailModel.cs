using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leo.Project.Portfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestEmailModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "RequestEmails");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RequestEmails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "RequestEmails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "RequestEmails");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RequestEmails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "RequestEmails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
