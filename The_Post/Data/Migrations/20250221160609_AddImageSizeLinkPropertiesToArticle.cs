using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace The_Post.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageSizeLinkPropertiesToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageLink",
                table: "Articles",
                newName: "ImageOriginalLink");

            migrationBuilder.AddColumn<string>(
                name: "ImageMediumLink",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageSmallLink",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageMediumLink",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ImageSmallLink",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "ImageOriginalLink",
                table: "Articles",
                newName: "ImageLink");
        }
    }
}
