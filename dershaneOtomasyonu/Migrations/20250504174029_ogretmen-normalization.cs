using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class ogretmennormalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OgrenciCevaplari_Kullanicilar_KullaniciId",
                table: "OgrenciCevaplari");

            migrationBuilder.DropForeignKey(
                name: "FK_Sinavlar_SinavKategorileri_SinavKategoriId",
                table: "Sinavlar");

            migrationBuilder.DropColumn(
                name: "OlusturmaTarihi",
                table: "OgrenciCevaplari");

            migrationBuilder.AddColumn<int>(
                name: "SinavId",
                table: "OgrenciCevaplari",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SinavSonuclari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    SinavId = table.Column<int>(type: "int", nullable: false),
                    ToplamDogrular = table.Column<int>(type: "int", nullable: false),
                    ToplamYanlislar = table.Column<int>(type: "int", nullable: false),
                    ToplamPuan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinavSonuclari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SinavSonuclari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SinavSonuclari_Sinavlar_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OgrenciCevaplari_SinavId",
                table: "OgrenciCevaplari",
                column: "SinavId");

            migrationBuilder.CreateIndex(
                name: "IX_SinavSonuclari_KullaniciId",
                table: "SinavSonuclari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_SinavSonuclari_SinavId",
                table: "SinavSonuclari",
                column: "SinavId");

            migrationBuilder.AddForeignKey(
                name: "FK_OgrenciCevaplari_Kullanicilar_KullaniciId",
                table: "OgrenciCevaplari",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OgrenciCevaplari_Sinavlar_SinavId",
                table: "OgrenciCevaplari",
                column: "SinavId",
                principalTable: "Sinavlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sinavlar_SinavKategorileri_SinavKategoriId",
                table: "Sinavlar",
                column: "SinavKategoriId",
                principalTable: "SinavKategorileri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OgrenciCevaplari_Kullanicilar_KullaniciId",
                table: "OgrenciCevaplari");

            migrationBuilder.DropForeignKey(
                name: "FK_OgrenciCevaplari_Sinavlar_SinavId",
                table: "OgrenciCevaplari");

            migrationBuilder.DropForeignKey(
                name: "FK_Sinavlar_SinavKategorileri_SinavKategoriId",
                table: "Sinavlar");

            migrationBuilder.DropTable(
                name: "SinavSonuclari");

            migrationBuilder.DropIndex(
                name: "IX_OgrenciCevaplari_SinavId",
                table: "OgrenciCevaplari");

            migrationBuilder.DropColumn(
                name: "SinavId",
                table: "OgrenciCevaplari");

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturmaTarihi",
                table: "OgrenciCevaplari",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_OgrenciCevaplari_Kullanicilar_KullaniciId",
                table: "OgrenciCevaplari",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sinavlar_SinavKategorileri_SinavKategoriId",
                table: "Sinavlar",
                column: "SinavKategoriId",
                principalTable: "SinavKategorileri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
