using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_URLs_FullUrl",
                table: "URLs");

            migrationBuilder.DropIndex(
                name: "IX_URLs_ShortUrl",
                table: "URLs");

            migrationBuilder.AlterColumn<string>(
                name: "ShortUrl",
                table: "URLs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FullUrl",
                table: "URLs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortUrl",
                table: "URLs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullUrl",
                table: "URLs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_URLs_FullUrl",
                table: "URLs",
                column: "FullUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_URLs_ShortUrl",
                table: "URLs",
                column: "ShortUrl",
                unique: true);
        }
    }
}
