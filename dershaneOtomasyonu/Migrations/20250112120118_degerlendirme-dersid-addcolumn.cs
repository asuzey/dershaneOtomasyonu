using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class degerlendirmedersidaddcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DersId",
                table: "Degerlendirmeler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_DersId",
                table: "Degerlendirmeler",
                column: "DersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Degerlendirmeler_Dersler_DersId",
                table: "Degerlendirmeler",
                column: "DersId",
                principalTable: "Dersler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Degerlendirmeler_Dersler_DersId",
                table: "Degerlendirmeler");

            migrationBuilder.DropIndex(
                name: "IX_Degerlendirmeler_DersId",
                table: "Degerlendirmeler");

            migrationBuilder.DropColumn(
                name: "DersId",
                table: "Degerlendirmeler");
        }
    }
}
