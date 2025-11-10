using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAthelteInjuries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImpactOnTraining",
                table: "AthleteInjuries",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Treatment",
                table: "AthleteInjuries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImpactOnTraining",
                table: "AthleteInjuries");

            migrationBuilder.DropColumn(
                name: "Treatment",
                table: "AthleteInjuries");
        }
    }
}
