using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rbac_IctJohor.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTemporaryAgencyAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgencyAddress",
                table: "Agencies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgencyAddress",
                table: "Agencies",
                type: "text",
                nullable: true);
        }
    }
}
