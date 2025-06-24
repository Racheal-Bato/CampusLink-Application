using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusLink_Application.Migrations
{
    /// <inheritdoc />
    public partial class ProfileImageUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePicturePath",
                table: "Students",
                newName: "ProfileImage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImage",
                table: "Students",
                newName: "ProfilePicturePath");
        }
    }
}
