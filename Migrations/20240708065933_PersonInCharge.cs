using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rbac_IctJohor.Migrations
{
    /// <inheritdoc />
    public partial class PersonInCharge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPersonInCharge",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AgencyAddress",
                table: "Agencies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPersonInCharge",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AgencyAddress",
                table: "Agencies");
        }
    }
}
