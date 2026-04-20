using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        EnsureSchemaCompatibility(db);

        if (db.Musteriler.Any() == false)
        {
            var m1 = new Musteri
            {
                AdSoyad = "Ahmet Yildiz",
                Telefon = "0555 111 22 33",
                Email = "ahmet@example.com",
                UyeHesabiVar = false,
                Sifre = string.Empty,
                KayitTarihi = DateTime.UtcNow.AddDays(-20)
            };

            var m2 = new Musteri
            {
                AdSoyad = "Elif Demir",
                Telefon = "0555 444 55 66",
                Email = "elif@example.com",
                UyeHesabiVar = false,
                Sifre = string.Empty,
                KayitTarihi = DateTime.UtcNow.AddDays(-18)
            };

            db.Musteriler.AddRange(m1, m2);
            db.SaveChanges();

            var c1 = new Cihaz
            {
                MusteriId = m1.Id,
                Marka = "Apple",
                Model = "iPhone 13",
                ArizaAciklama = "Ekran kirik ve dokunmatik gec tepki veriyor."
            };

            var c2 = new Cihaz
            {
                MusteriId = m2.Id,
                Marka = "Samsung",
                Model = "Galaxy S22",
                ArizaAciklama = "Sarj soketi temassizlik yapiyor."
            };

            db.Cihazlar.AddRange(c1, c2);
            db.SaveChanges();

            var s1 = new ServisKaydi
            {
                CihazId = c1.Id,
                Tarih = DateTime.UtcNow.AddDays(-3),
                Durum = "Islemde",
                ToplamFiyat = 2450m
            };

            var s2 = new ServisKaydi
            {
                CihazId = c2.Id,
                Tarih = DateTime.UtcNow.AddDays(-1),
                Durum = "Bekliyor",
                ToplamFiyat = 0m
            };

            db.ServisKayitlari.AddRange(s1, s2);
            db.SaveChanges();

            var ekranDegisimi = db.Islemler.FirstOrDefault(x => x.Ad == "Ekran Degisimi");
            if (ekranDegisimi != null)
            {
                db.ServisIslemler.Add(new ServisIslem
                {
                    ServisKaydiId = s1.Id,
                    IslemId = ekranDegisimi.Id
                });

                db.SaveChanges();
            }
        }
    }

    private static void EnsureSchemaCompatibility(AppDbContext db)
    {
        using var connection = new SqliteConnection(db.Database.GetConnectionString());
        connection.Open();

        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info('Musteriler');";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(reader.GetString(1));
            }
        }

        if (columns.Contains("Sifre") == false)
        {
            Execute(connection, "ALTER TABLE Musteriler ADD COLUMN Sifre TEXT NOT NULL DEFAULT '';");
        }

        if (columns.Contains("UyeHesabiVar") == false)
        {
            Execute(connection, "ALTER TABLE Musteriler ADD COLUMN UyeHesabiVar INTEGER NOT NULL DEFAULT 0;");
        }

        if (columns.Contains("KayitTarihi") == false)
        {
            Execute(connection, "ALTER TABLE Musteriler ADD COLUMN KayitTarihi TEXT NOT NULL DEFAULT '2000-01-01T00:00:00Z';");
        }

        if (columns.Contains("SonGirisTarihi") == false)
        {
            Execute(connection, "ALTER TABLE Musteriler ADD COLUMN SonGirisTarihi TEXT NULL;");
        }

        Execute(connection, "CREATE UNIQUE INDEX IF NOT EXISTS IX_Musteriler_Email ON Musteriler (Email);");
        Execute(connection, "UPDATE Musteriler SET Sifre = '' WHERE Sifre IS NULL;");
        Execute(connection, "UPDATE Musteriler SET KayitTarihi = datetime('now') WHERE KayitTarihi IS NULL;");
    }

    private static void Execute(SqliteConnection connection, string sql)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }
}
