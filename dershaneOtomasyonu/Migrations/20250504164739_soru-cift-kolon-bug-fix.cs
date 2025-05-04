using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class soruciftkolonbugfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sorular_SinavDersKonulari_SinavDersKonuId1",
                table: "Sorular");

            migrationBuilder.DropForeignKey(
                name: "FK_Sorular_Sinavlar_SinavDersKonuId",
                table: "Sorular");

            migrationBuilder.DropForeignKey(
                name: "FK_Sorular_SinifSeviyeleri_SinifSeviyeId1",
                table: "Sorular");

            migrationBuilder.DropIndex(
                name: "IX_Sorular_SinavDersKonuId1",
                table: "Sorular");

            migrationBuilder.DropIndex(
                name: "IX_Sorular_SinifSeviyeId1",
                table: "Sorular");

            migrationBuilder.DropColumn(
                name: "SinavDersKonuId1",
                table: "Sorular");

            migrationBuilder.DropColumn(
                name: "SinifSeviyeId1",
                table: "Sorular");

            migrationBuilder.AddForeignKey(
                name: "FK_Sorular_SinavDersKonulari_SinavDersKonuId",
                table: "Sorular",
                column: "SinavDersKonuId",
                principalTable: "SinavDersKonulari",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sorular_SinavDersKonulari_SinavDersKonuId",
                table: "Sorular");

            migrationBuilder.AddColumn<int>(
                name: "SinavDersKonuId1",
                table: "Sorular",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SinifSeviyeId1",
                table: "Sorular",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_SinavDersKonuId1",
                table: "Sorular",
                column: "SinavDersKonuId1");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_SinifSeviyeId1",
                table: "Sorular",
                column: "SinifSeviyeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Sorular_SinavDersKonulari_SinavDersKonuId1",
                table: "Sorular",
                column: "SinavDersKonuId1",
                principalTable: "SinavDersKonulari",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sorular_Sinavlar_SinavDersKonuId",
                table: "Sorular",
                column: "SinavDersKonuId",
                principalTable: "Sinavlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sorular_SinifSeviyeleri_SinifSeviyeId1",
                table: "Sorular",
                column: "SinifSeviyeId1",
                principalTable: "SinifSeviyeleri",
                principalColumn: "Id");
        }
    }
}
