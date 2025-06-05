using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriversLicenseTestWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Added_PageIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageIndex",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageIndex",
                table: "Questions");
        }
    }
}
