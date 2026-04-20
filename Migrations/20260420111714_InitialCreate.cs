using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace projem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adminler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciAdi = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Sifre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adminler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Islemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Fiyat = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Islemler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Musteriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdSoyad = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Telefon = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Sifre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UyeHesabiVar = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    KayitTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musteriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cihazlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MusteriId = table.Column<int>(type: "INTEGER", nullable: false),
                    Marka = table.Column<string>(type: "TEXT", maxLength: 70, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 90, nullable: false),
                    ArizaAciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cihazlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cihazlar_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServisKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CihazId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Durum = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ToplamFiyat = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServisKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServisKayitlari_Cihazlar_CihazId",
                        column: x => x.CihazId,
                        principalTable: "Cihazlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServisIslemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServisKaydiId = table.Column<int>(type: "INTEGER", nullable: false),
                    IslemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServisIslemler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServisIslemler_Islemler_IslemId",
                        column: x => x.IslemId,
                        principalTable: "Islemler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServisIslemler_ServisKayitlari_ServisKaydiId",
                        column: x => x.ServisKaydiId,
                        principalTable: "ServisKayitlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Adminler",
                columns: new[] { "Id", "KullaniciAdi", "Sifre" },
                values: new object[] { 1, "admin", "12345" });

            migrationBuilder.InsertData(
                table: "Islemler",
                columns: new[] { "Id", "Ad", "Fiyat" },
                values: new object[,]
                {
                    { 1, "Ekran Degisimi", 2450m },
                    { 2, "Batarya Degisimi", 1250m },
                    { 3, "Soket Tamiri", 900m },
                    { 4, "Genel Bakim", 650m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cihazlar_MusteriId",
                table: "Cihazlar",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_Musteriler_Email",
                table: "Musteriler",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServisIslemler_IslemId",
                table: "ServisIslemler",
                column: "IslemId");

            migrationBuilder.CreateIndex(
                name: "IX_ServisIslemler_ServisKaydiId_IslemId",
                table: "ServisIslemler",
                columns: new[] { "ServisKaydiId", "IslemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServisKayitlari_CihazId",
                table: "ServisKayitlari",
                column: "CihazId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adminler");

            migrationBuilder.DropTable(
                name: "ServisIslemler");

            migrationBuilder.DropTable(
                name: "Islemler");

            migrationBuilder.DropTable(
                name: "ServisKayitlari");

            migrationBuilder.DropTable(
                name: "Cihazlar");

            migrationBuilder.DropTable(
                name: "Musteriler");
        }
    }
}
