using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projem.Migrations
{
    /// <inheritdoc />
    public partial class AddMusteriBildirimleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MusteriBildirimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MusteriId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServisKaydiId = table.Column<int>(type: "INTEGER", nullable: true),
                    Tur = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Genel"),
                    Baslik = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Mesaj = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Okundu = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OkunmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusteriBildirimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusteriBildirimleri_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusteriBildirimleri_ServisKayitlari_ServisKaydiId",
                        column: x => x.ServisKaydiId,
                        principalTable: "ServisKayitlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusteriBildirimleri_MusteriId_Okundu_OlusturmaTarihi",
                table: "MusteriBildirimleri",
                columns: new[] { "MusteriId", "Okundu", "OlusturmaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_MusteriBildirimleri_ServisKaydiId_Tur",
                table: "MusteriBildirimleri",
                columns: new[] { "ServisKaydiId", "Tur" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusteriBildirimleri");
        }
    }
}
