using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CommunicationsApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedChannelRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Users_OwnerId",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_OwnerId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Channels");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "ChannelMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChannelRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelRoles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ChannelRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Owner" },
                    { 2, "Member" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMembers_RoleId",
                table: "ChannelMembers",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMembers_ChannelRoles_RoleId",
                table: "ChannelMembers",
                column: "RoleId",
                principalTable: "ChannelRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMembers_ChannelRoles_RoleId",
                table: "ChannelMembers");

            migrationBuilder.DropTable(
                name: "ChannelRoles");

            migrationBuilder.DropIndex(
                name: "IX_ChannelMembers_RoleId",
                table: "ChannelMembers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "ChannelMembers");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Channels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_OwnerId",
                table: "Channels",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Users_OwnerId",
                table: "Channels",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
