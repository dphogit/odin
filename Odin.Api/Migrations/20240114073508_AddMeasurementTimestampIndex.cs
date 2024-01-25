using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Odin.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMeasurementTimestampIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Temperatures_Timestamp",
                table: "Temperatures",
                column: "Timestamp",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Temperatures_Timestamp",
                table: "Temperatures");
        }
    }
}
