using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class derskayitdersidaddcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DersId",
                table: "DersKayitlari",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DersKayitlari_DersId",
                table: "DersKayitlari",
                column: "DersId");

            migrationBuilder.AddForeignKey(
                name: "FK_DersKayitlari_Dersler_DersId",
                table: "DersKayitlari",
                column: "DersId",
                principalTable: "Dersler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DersKayitlari_Dersler_DersId",
                table: "DersKayitlari");

            migrationBuilder.DropIndex(
                name: "IX_DersKayitlari_DersId",
                table: "DersKayitlari");

            migrationBuilder.DropColumn(
                name: "DersId",
                table: "DersKayitlari");
        }
    }
}
