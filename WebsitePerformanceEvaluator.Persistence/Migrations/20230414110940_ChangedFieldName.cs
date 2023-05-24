using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsitePerformanceEvaluator.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFieldName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Link",
                table: "LinkPerformances",
                newName: "Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "LinkPerformances",
                newName: "Link");
        }
    }
}
