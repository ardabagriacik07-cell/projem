using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projem.Migrations
{
    /// <inheritdoc />
    public partial class AddIslemFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "Islemler",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Kategori",
                table: "Islemler",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaksimumFiyat",
                table: "Islemler",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumFiyat",
                table: "Islemler",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Islemler",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Aciklama", "Kategori", "MaksimumFiyat", "MinimumFiyat" },
                values: new object[] { null, "Genel", 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Islemler",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Aciklama", "Kategori", "MaksimumFiyat", "MinimumFiyat" },
                values: new object[] { null, "Genel", 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Islemler",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Aciklama", "Kategori", "MaksimumFiyat", "MinimumFiyat" },
                values: new object[] { null, "Genel", 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Islemler",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Aciklama", "Kategori", "MaksimumFiyat", "MinimumFiyat" },
                values: new object[] { null, "Genel", 0m, 0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "Islemler");

            migrationBuilder.DropColumn(
                name: "Kategori",
                table: "Islemler");

            migrationBuilder.DropColumn(
                name: "MaksimumFiyat",
                table: "Islemler");

            migrationBuilder.DropColumn(
                name: "MinimumFiyat",
                table: "Islemler");
        }
    }
}
