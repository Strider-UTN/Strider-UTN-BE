using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmailUsernameUniqueWithUserType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_UserType",
                table: "Users",
                columns: new[] { "Email", "UserType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username_UserType",
                table: "Users",
                columns: new[] { "Username", "UserType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_UserType",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username_UserType",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }
    }
}
