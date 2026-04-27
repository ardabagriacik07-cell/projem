using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projem.Migrations
{
    /// <inheritdoc />
    public partial class AddUyeSifreSifirlama : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SifreSifirlamaKodSonTarih",
                table: "Musteriler",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SifreSifirlamaKodu",
                table: "Musteriler",
                type: "TEXT",
                maxLength: 6,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SifreSifirlamaKodSonTarih",
                table: "Musteriler");

            migrationBuilder.DropColumn(
                name: "SifreSifirlamaKodu",
                table: "Musteriler");
        }
    }
}
