using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rbac_IctJohor.Migrations
{
    /// <inheritdoc />
    public partial class AgencyContactNoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgencyContactNo",
                table: "Agencies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgencyContactNo",
                table: "Agencies");
        }
    }
}
