using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class dosyaolusturucuaddcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OlusturucuId",
                table: "Dosyalar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Dosyalar_OlusturucuId",
                table: "Dosyalar",
                column: "OlusturucuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dosyalar_Kullanicilar_OlusturucuId",
                table: "Dosyalar",
                column: "OlusturucuId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dosyalar_Kullanicilar_OlusturucuId",
                table: "Dosyalar");

            migrationBuilder.DropIndex(
                name: "IX_Dosyalar_OlusturucuId",
                table: "Dosyalar");

            migrationBuilder.DropColumn(
                name: "OlusturucuId",
                table: "Dosyalar");
        }
    }
}
