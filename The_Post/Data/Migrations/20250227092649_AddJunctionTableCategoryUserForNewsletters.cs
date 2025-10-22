using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace The_Post.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJunctionTableCategoryUserForNewsletters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageMediumLink",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ImageSmallLink",
                table: "Articles");

            migrationBuilder.AddColumn<bool>(
                name: "EditorsChoiceNewsletter",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CategoryUser",
                columns: table => new
                {
                    NewsletterCategoriesId = table.Column<int>(type: "int", nullable: false),
                    NewsletterUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryUser", x => new { x.NewsletterCategoriesId, x.NewsletterUsersId });
                    table.ForeignKey(
                        name: "FK_CategoryUser_AspNetUsers_NewsletterUsersId",
                        column: x => x.NewsletterUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryUser_Categories_NewsletterCategoriesId",
                        column: x => x.NewsletterCategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryUser_NewsletterUsersId",
                table: "CategoryUser",
                column: "NewsletterUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryUser");

            migrationBuilder.DropColumn(
                name: "EditorsChoiceNewsletter",
                table: "AspNetUsers");

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
    }
}
