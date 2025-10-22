using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace The_Post.Data.Migrations
{
    /// <inheritdoc />
    public partial class addpropertyinArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EditorsChoice",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditorsChoice",
                table: "Articles");
        }
    }
}
