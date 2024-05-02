using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunicationsApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedChannelMemberUserFKIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMembers_Users_MembersId",
                table: "ChannelMembers");

            migrationBuilder.DropIndex(
                name: "IX_ChannelMembers_MembersId",
                table: "ChannelMembers");

            migrationBuilder.DropColumn(
                name: "MembersId",
                table: "ChannelMembers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembersId",
                table: "ChannelMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMembers_MembersId",
                table: "ChannelMembers",
                column: "MembersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMembers_Users_MembersId",
                table: "ChannelMembers",
                column: "MembersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
