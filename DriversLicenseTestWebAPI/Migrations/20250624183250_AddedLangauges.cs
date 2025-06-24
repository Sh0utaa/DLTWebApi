using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriversLicenseTestWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedLangauges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Questions");
        }
    }
}
