using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class esinav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SinifSeviyeId",
                table: "Siniflar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SinavKategorileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VarsayilanSure = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinavKategorileri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SinifSeviyeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Seviye = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinifSeviyeleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SinavDersleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SinavKategoriId = table.Column<int>(type: "int", nullable: false),
                    SoruSayisi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinavDersleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SinavDersleri_SinavKategorileri_SinavKategoriId",
                        column: x => x.SinavKategoriId,
                        principalTable: "SinavKategorileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sinavlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SinavKategoriId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sure = table.Column<int>(type: "int", nullable: false),
                    OlusturucuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sinavlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sinavlar_Kullanicilar_OlusturucuId",
                        column: x => x.OlusturucuId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sinavlar_SinavKategorileri_SinavKategoriId",
                        column: x => x.SinavKategoriId,
                        principalTable: "SinavKategorileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SinavDersKonulari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SinavDersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinavDersKonulari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SinavDersKonulari_SinavDersleri_SinavDersId",
                        column: x => x.SinavDersId,
                        principalTable: "SinavDersleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kopyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SinavId = table.Column<int>(type: "int", nullable: false),
                    DosyaId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kopyalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kopyalar_Dosyalar_DosyaId",
                        column: x => x.DosyaId,
                        principalTable: "Dosyalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kopyalar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kopyalar_Sinavlar_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OgrenciSinavlari",
                columns: table => new
                {
                    SinavId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgrenciSinavlari", x => new { x.KullaniciId, x.SinavId });
                    table.ForeignKey(
                        name: "FK_OgrenciSinavlari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OgrenciSinavlari_Sinavlar_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sorular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoruMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SinifSeviyeId = table.Column<int>(type: "int", nullable: false),
                    YildizSeviyesi = table.Column<short>(type: "smallint", nullable: false),
                    SinavDersKonuId = table.Column<int>(type: "int", nullable: false),
                    SecenekSayisi = table.Column<int>(type: "int", nullable: false, defaultValue: 4),
                    SinavDersKonuId1 = table.Column<int>(type: "int", nullable: false),
                    SinavId = table.Column<int>(type: "int", nullable: true),
                    SinifSeviyeId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sorular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sorular_SinavDersKonulari_SinavDersKonuId1",
                        column: x => x.SinavDersKonuId1,
                        principalTable: "SinavDersKonulari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sorular_Sinavlar_SinavDersKonuId",
                        column: x => x.SinavDersKonuId,
                        principalTable: "Sinavlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sorular_Sinavlar_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavlar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sorular_SinifSeviyeleri_SinifSeviyeId",
                        column: x => x.SinifSeviyeId,
                        principalTable: "SinifSeviyeleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sorular_SinifSeviyeleri_SinifSeviyeId1",
                        column: x => x.SinifSeviyeId1,
                        principalTable: "SinifSeviyeleri",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Secenekler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoruId = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secenekler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Secenekler_Sorular_SoruId",
                        column: x => x.SoruId,
                        principalTable: "Sorular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SinavSorulari",
                columns: table => new
                {
                    SinavId = table.Column<int>(type: "int", nullable: false),
                    SoruId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinavSorulari", x => new { x.SinavId, x.SoruId });
                    table.ForeignKey(
                        name: "FK_SinavSorulari_Sinavlar_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SinavSorulari_Sorular_SoruId",
                        column: x => x.SoruId,
                        principalTable: "Sorular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OgrenciCevaplari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SecenekId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Sure = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgrenciCevaplari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OgrenciCevaplari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OgrenciCevaplari_Secenekler_SecenekId",
                        column: x => x.SecenekId,
                        principalTable: "Secenekler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Siniflar_SinifSeviyeId",
                table: "Siniflar",
                column: "SinifSeviyeId");

            migrationBuilder.CreateIndex(
                name: "IX_Kopyalar_DosyaId",
                table: "Kopyalar",
                column: "DosyaId");

            migrationBuilder.CreateIndex(
                name: "IX_Kopyalar_KullaniciId",
                table: "Kopyalar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Kopyalar_SinavId",
                table: "Kopyalar",
                column: "SinavId");

            migrationBuilder.CreateIndex(
                name: "IX_OgrenciCevaplari_KullaniciId",
                table: "OgrenciCevaplari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_OgrenciCevaplari_SecenekId",
                table: "OgrenciCevaplari",
                column: "SecenekId");

            migrationBuilder.CreateIndex(
                name: "IX_OgrenciSinavlari_SinavId",
                table: "OgrenciSinavlari",
                column: "SinavId");

            migrationBuilder.CreateIndex(
                name: "IX_Secenekler_SoruId",
                table: "Secenekler",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_SinavDersKonulari_SinavDersId",
                table: "SinavDersKonulari",
                column: "SinavDersId");

            migrationBuilder.CreateIndex(
                name: "IX_SinavDersleri_SinavKategoriId",
                table: "SinavDersleri",
                column: "SinavKategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Sinavlar_OlusturucuId",
                table: "Sinavlar",
                column: "OlusturucuId");

            migrationBuilder.CreateIndex(
                name: "IX_Sinavlar_SinavKategoriId",
                table: "Sinavlar",
                column: "SinavKategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_SinavSorulari_SoruId",
                table: "SinavSorulari",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_SinavDersKonuId",
                table: "Sorular",
                column: "SinavDersKonuId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_SinavDersKonuId1",
                table: "Sorular",
                column: "SinavDersKonuId1");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_SinavId",
                table: "Sorular",
                column: "SinavId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_SinifSeviyeId",
                table: "Sorular",
                column: "SinifSeviyeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_SinifSeviyeId1",
                table: "Sorular",
                column: "SinifSeviyeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Siniflar_SinifSeviyeleri_SinifSeviyeId",
                table: "Siniflar",
                column: "SinifSeviyeId",
                principalTable: "SinifSeviyeleri",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Siniflar_SinifSeviyeleri_SinifSeviyeId",
                table: "Siniflar");

            migrationBuilder.DropTable(
                name: "Kopyalar");

            migrationBuilder.DropTable(
                name: "OgrenciCevaplari");

            migrationBuilder.DropTable(
                name: "OgrenciSinavlari");

            migrationBuilder.DropTable(
                name: "SinavSorulari");

            migrationBuilder.DropTable(
                name: "Secenekler");

            migrationBuilder.DropTable(
                name: "Sorular");

            migrationBuilder.DropTable(
                name: "SinavDersKonulari");

            migrationBuilder.DropTable(
                name: "Sinavlar");

            migrationBuilder.DropTable(
                name: "SinifSeviyeleri");

            migrationBuilder.DropTable(
                name: "SinavDersleri");

            migrationBuilder.DropTable(
                name: "SinavKategorileri");

            migrationBuilder.DropIndex(
                name: "IX_Siniflar_SinifSeviyeId",
                table: "Siniflar");

            migrationBuilder.DropColumn(
                name: "SinifSeviyeId",
                table: "Siniflar");
        }
    }
}
