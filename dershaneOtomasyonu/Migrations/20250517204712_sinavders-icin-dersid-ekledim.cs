using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dershaneOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class sinavdersicindersidekledim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DersId",
                table: "SinavDersleri",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SinavDersleri_DersId",
                table: "SinavDersleri",
                column: "DersId");

            migrationBuilder.AddForeignKey(
                name: "FK_SinavDersleri_Dersler_DersId",
                table: "SinavDersleri",
                column: "DersId",
                principalTable: "Dersler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SinavDersleri_Dersler_DersId",
                table: "SinavDersleri");

            migrationBuilder.DropIndex(
                name: "IX_SinavDersleri_DersId",
                table: "SinavDersleri");

            migrationBuilder.DropColumn(
                name: "DersId",
                table: "SinavDersleri");
        }
    }
}
