using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projem.Migrations
{
    /// <inheritdoc />
    public partial class AddUyeYorumlari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UyeYorumlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MusteriId = table.Column<int>(type: "INTEGER", nullable: false),
                    Baslik = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Mesaj = table.Column<string>(type: "TEXT", maxLength: 800, nullable: false),
                    Puan = table.Column<int>(type: "INTEGER", nullable: false),
                    YoneticiGordu = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UyeYorumlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UyeYorumlari_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UyeYorumlari_MusteriId_OlusturmaTarihi",
                table: "UyeYorumlari",
                columns: new[] { "MusteriId", "OlusturmaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_UyeYorumlari_YoneticiGordu_OlusturmaTarihi",
                table: "UyeYorumlari",
                columns: new[] { "YoneticiGordu", "OlusturmaTarihi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UyeYorumlari");
        }
    }
}
