using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projem.Migrations
{
    /// <inheritdoc />
    public partial class AddServisFiyatOnayi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FiyatCevapTarihi",
                table: "ServisKayitlari",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiyatOnayDurumu",
                table: "ServisKayitlari",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "Onay Gerekmez");

            migrationBuilder.AddColumn<DateTime>(
                name: "FiyatOnayTarihi",
                table: "ServisKayitlari",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FiyatCevapTarihi",
                table: "ServisKayitlari");

            migrationBuilder.DropColumn(
                name: "FiyatOnayDurumu",
                table: "ServisKayitlari");

            migrationBuilder.DropColumn(
                name: "FiyatOnayTarihi",
                table: "ServisKayitlari");
        }
    }
}
