using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsitePerformanceEvaluator.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCascadeToNoActionInLinkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkPerformances_Links_LinkId",
                table: "LinkPerformances");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkPerformances_Links_LinkId",
                table: "LinkPerformances",
                column: "LinkId",
                principalTable: "Links",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkPerformances_Links_LinkId",
                table: "LinkPerformances");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkPerformances_Links_LinkId",
                table: "LinkPerformances",
                column: "LinkId",
                principalTable: "Links",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
