using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace The_Post.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWeatherCitiesStringToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WeatherCities",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeatherCities",
                table: "AspNetUsers");
        }
    }
}
