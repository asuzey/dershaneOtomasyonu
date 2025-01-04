using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class kullanicisinifdelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciSiniflari");

            migrationBuilder.AddColumn<int>(
                name: "SinifId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_SinifId",
                table: "Kullanicilar",
                column: "SinifId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Siniflar_SinifId",
                table: "Kullanicilar",
                column: "SinifId",
                principalTable: "Siniflar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Siniflar_SinifId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_SinifId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "SinifId",
                table: "Kullanicilar");

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

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciSiniflari_SinifId",
                table: "KullaniciSiniflari",
                column: "SinifId");
        }
    }
}
