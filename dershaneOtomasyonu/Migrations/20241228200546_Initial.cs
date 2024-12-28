using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dersler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dersler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolAdi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Siniflar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kodu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siniflar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Soyadi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tcno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Roller_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Degerlendirmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Puan = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degerlendirmeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Degerlendirmeler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DersKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SinifId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Oda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesajlar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Durum = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DersKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DersKayitlari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DersKayitlari_Siniflar_SinifId",
                        column: x => x.SinifId,
                        principalTable: "Siniflar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gorusmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Oda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesajlar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KatilimciId = table.Column<int>(type: "int", nullable: false),
                    OlusturucuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gorusmeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gorusmeler_Kullanicilar_KatilimciId",
                        column: x => x.KatilimciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gorusmeler_Kullanicilar_OlusturucuId",
                        column: x => x.OlusturucuId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciDersleri",
                columns: table => new
                {
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    DersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciDersleri", x => new { x.KullaniciId, x.DersId });
                    table.ForeignKey(
                        name: "FK_KullaniciDersleri_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciDersleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciNotlari",
                columns: table => new
                {
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    NotId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciNotlari", x => new { x.KullaniciId, x.NotId });
                    table.ForeignKey(
                        name: "FK_KullaniciNotlari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciNotlari_Notlar_NotId",
                        column: x => x.NotId,
                        principalTable: "Notlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciSiniflari",
                columns: table => new
                {
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    SinifId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciSiniflari", x => new { x.KullaniciId, x.SinifId });
                    table.ForeignKey(
                        name: "FK_KullaniciSiniflari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciSiniflari_Siniflar_SinifId",
                        column: x => x.SinifId,
                        principalTable: "Siniflar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Yoklamalar",
                columns: table => new
                {
                    DersKayitId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yoklamalar", x => new { x.DersKayitId, x.KullaniciId });
                    table.ForeignKey(
                        name: "FK_Yoklamalar_DersKayitlari_DersKayitId",
                        column: x => x.DersKayitId,
                        principalTable: "DersKayitlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Yoklamalar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_KullaniciId",
                table: "Degerlendirmeler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_DersKayitlari_KullaniciId",
                table: "DersKayitlari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_DersKayitlari_SinifId",
                table: "DersKayitlari",
                column: "SinifId");

            migrationBuilder.CreateIndex(
                name: "IX_Gorusmeler_KatilimciId",
                table: "Gorusmeler",
                column: "KatilimciId");

            migrationBuilder.CreateIndex(
                name: "IX_Gorusmeler_OlusturucuId",
                table: "Gorusmeler",
                column: "OlusturucuId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersleri_DersId",
                table: "KullaniciDersleri",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_RoleId",
                table: "Kullanicilar",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciNotlari_NotId",
                table: "KullaniciNotlari",
                column: "NotId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciSiniflari_SinifId",
                table: "KullaniciSiniflari",
                column: "SinifId");

            migrationBuilder.CreateIndex(
                name: "IX_Yoklamalar_KullaniciId",
                table: "Yoklamalar",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Degerlendirmeler");

            migrationBuilder.DropTable(
                name: "Gorusmeler");

            migrationBuilder.DropTable(
                name: "KullaniciDersleri");

            migrationBuilder.DropTable(
                name: "KullaniciNotlari");

            migrationBuilder.DropTable(
                name: "KullaniciSiniflari");

            migrationBuilder.DropTable(
                name: "Yoklamalar");

            migrationBuilder.DropTable(
                name: "Dersler");

            migrationBuilder.DropTable(
                name: "Notlar");

            migrationBuilder.DropTable(
                name: "DersKayitlari");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Siniflar");

            migrationBuilder.DropTable(
                name: "Roller");
        }
    }
}
