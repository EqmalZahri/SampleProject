using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rbac_IctJohor.Migrations
{
    /// <inheritdoc />
    public partial class AgencyAddressField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Agencies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Agencies");
        }
    }
}
