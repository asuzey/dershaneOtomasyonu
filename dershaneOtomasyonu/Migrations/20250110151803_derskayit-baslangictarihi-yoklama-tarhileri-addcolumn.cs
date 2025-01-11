using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class derskayitbaslangictarihiyoklamatarhileriaddcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AyrilmaTarihi",
                table: "Yoklamalar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "KatilmaTarihi",
                table: "Yoklamalar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BaslangicTarihi",
                table: "DersKayitlari",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AyrilmaTarihi",
                table: "Yoklamalar");

            migrationBuilder.DropColumn(
                name: "KatilmaTarihi",
                table: "Yoklamalar");

            migrationBuilder.DropColumn(
                name: "BaslangicTarihi",
                table: "DersKayitlari");
        }
    }
}
