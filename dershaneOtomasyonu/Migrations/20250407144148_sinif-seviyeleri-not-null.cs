using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class sinifseviyelerinotnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Siniflar_SinifSeviyeleri_SinifSeviyeId",
                table: "Siniflar");

            migrationBuilder.AlterColumn<int>(
                name: "SinifSeviyeId",
                table: "Siniflar",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Siniflar_SinifSeviyeleri_SinifSeviyeId",
                table: "Siniflar",
                column: "SinifSeviyeId",
                principalTable: "SinifSeviyeleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Siniflar_SinifSeviyeleri_SinifSeviyeId",
                table: "Siniflar");

            migrationBuilder.AlterColumn<int>(
                name: "SinifSeviyeId",
                table: "Siniflar",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Siniflar_SinifSeviyeleri_SinifSeviyeId",
                table: "Siniflar",
                column: "SinifSeviyeId",
                principalTable: "SinifSeviyeleri",
                principalColumn: "Id");
        }
    }
}
