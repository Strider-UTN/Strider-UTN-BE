using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalInformationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasHealthInsurance",
                table: "Users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthInsuranceMemberNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthInsuranceProvider",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckupDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MedicalClearanceExpiryDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasHealthInsurance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HealthInsuranceMemberNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HealthInsuranceProvider",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastCheckupDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MedicalClearanceExpiryDate",
                table: "Users");
        }
    }
}
