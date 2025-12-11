using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentoPlus_S.A.S.ll.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "Employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "Employees");
        }
    }
}
