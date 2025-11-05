using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StriderWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageToTrainingGroupMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvitationMessage",
                table: "TrainingGroupMembers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitationMessage",
                table: "TrainingGroupMembers");
        }
    }
}
