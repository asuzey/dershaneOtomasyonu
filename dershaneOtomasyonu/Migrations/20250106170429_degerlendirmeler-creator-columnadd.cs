using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class degerlendirmelercreatorcolumnadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Degerlendirmeler_Kullanicilar_KullaniciId",
                table: "Degerlendirmeler");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Degerlendirmeler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_CreatorId",
                table: "Degerlendirmeler",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Degerlendirmeler_Kullanicilar_CreatorId",
                table: "Degerlendirmeler",
                column: "CreatorId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Degerlendirmeler_Kullanicilar_KullaniciId",
                table: "Degerlendirmeler",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Degerlendirmeler_Kullanicilar_CreatorId",
                table: "Degerlendirmeler");

            migrationBuilder.DropForeignKey(
                name: "FK_Degerlendirmeler_Kullanicilar_KullaniciId",
                table: "Degerlendirmeler");

            migrationBuilder.DropIndex(
                name: "IX_Degerlendirmeler_CreatorId",
                table: "Degerlendirmeler");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Degerlendirmeler");

            migrationBuilder.AddForeignKey(
                name: "FK_Degerlendirmeler_Kullanicilar_KullaniciId",
                table: "Degerlendirmeler",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
