using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillSwapBD.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionAndApprovalFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApprovalMessageShown",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalMessageShown",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "AspNetUsers");
        }
    }
}
