using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class FixPeriodRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Microcycles_Periods_PeriodId",
                table: "Microcycles");

            migrationBuilder.DropIndex(
                name: "IX_Microcycles_PeriodId",
                table: "Microcycles");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Microcycles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodId",
                table: "Microcycles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Microcycles_PeriodId",
                table: "Microcycles",
                column: "PeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Microcycles_Periods_PeriodId",
                table: "Microcycles",
                column: "PeriodId",
                principalTable: "Periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
