using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTAlumniConnect.DataTier.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Embedding",
                table: "CV",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "CV");
        }
    }
}
