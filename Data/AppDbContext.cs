using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Musteri> Musteriler => Set<Musteri>();
    public DbSet<Cihaz> Cihazlar => Set<Cihaz>();
    public DbSet<ServisKaydi> ServisKayitlari => Set<ServisKaydi>();
    public DbSet<Islem> Islemler => Set<Islem>();
    public DbSet<ServisIslem> ServisIslemler => Set<ServisIslem>();
    public DbSet<Admin> Adminler => Set<Admin>();
    public DbSet<MusteriBildirim> MusteriBildirimleri => Set<MusteriBildirim>();
    public DbSet<UyeYorum> UyeYorumlari => Set<UyeYorum>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Musteri>()
            .HasMany(x => x.Cihazlar)
            .WithOne(x => x.Musteri)
            .HasForeignKey(x => x.MusteriId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Musteri>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Musteri>()
            .Property(x => x.UyeHesabiVar)
            .HasDefaultValue(false);

        modelBuilder.Entity<MusteriBildirim>()
            .HasOne(x => x.Musteri)
            .WithMany(x => x.Bildirimler)
            .HasForeignKey(x => x.MusteriId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MusteriBildirim>()
            .HasOne(x => x.ServisKaydi)
            .WithMany(x => x.Bildirimler)
            .HasForeignKey(x => x.ServisKaydiId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MusteriBildirim>()
            .Property(x => x.Tur)
            .HasDefaultValue("Genel");

        modelBuilder.Entity<MusteriBildirim>()
            .Property(x => x.Okundu)
            .HasDefaultValue(false);

        modelBuilder.Entity<MusteriBildirim>()
            .HasIndex(x => new { x.MusteriId, x.Okundu, x.OlusturmaTarihi });

        modelBuilder.Entity<MusteriBildirim>()
            .HasIndex(x => new { x.ServisKaydiId, x.Tur });

        modelBuilder.Entity<UyeYorum>()
            .HasOne(x => x.Musteri)
            .WithMany(x => x.Yorumlar)
            .HasForeignKey(x => x.MusteriId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UyeYorum>()
            .HasIndex(x => new { x.MusteriId, x.OlusturmaTarihi });

        modelBuilder.Entity<UyeYorum>()
            .HasIndex(x => new { x.YoneticiGordu, x.OlusturmaTarihi });

        modelBuilder.Entity<Cihaz>()
            .HasMany(x => x.ServisKayitlari)
            .WithOne(x => x.Cihaz)
            .HasForeignKey(x => x.CihazId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServisKaydi>()
            .Property(x => x.FiyatOnayDurumu)
            .HasDefaultValue("Onay Gerekmez");

        modelBuilder.Entity<ServisIslem>()
            .HasOne(x => x.ServisKaydi)
            .WithMany(x => x.ServisIslemler)
            .HasForeignKey(x => x.ServisKaydiId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServisIslem>()
            .HasOne(x => x.Islem)
            .WithMany(x => x.ServisIslemler)
            .HasForeignKey(x => x.IslemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServisIslem>()
            .HasIndex(x => new { x.ServisKaydiId, x.IslemId })
            .IsUnique();

        modelBuilder.Entity<Admin>().HasData(new Admin
        {
            Id = 1,
            KullaniciAdi = "admin",
            Sifre = "12345"
        });

        modelBuilder.Entity<Islem>().HasData(
            new Islem { Id = 1, Ad = "Ekran Değişimi", Kategori = "Telefon", MinimumFiyat = 1800m, MaksimumFiyat = 6500m, Fiyat = 4150m, Aciklama = "Ön cam sağlamsa komple panel değişimi gerekebilir; marka ve OLED/AMOLED yapısına göre fiyat değişir." },
            new Islem { Id = 2, Ad = "Batarya Değişimi", Kategori = "Telefon", MinimumFiyat = 900m, MaksimumFiyat = 3200m, Fiyat = 2050m, Aciklama = "Pil sağlığı düşen veya şişen telefonlarda parça kalitesine göre fiyat aralığı değişir." },
            new Islem { Id = 3, Ad = "Soket Tamiri", Kategori = "Telefon", MinimumFiyat = 700m, MaksimumFiyat = 2200m, Fiyat = 1450m, Aciklama = "Şarj almama ve temassızlık sorunlarında soket temizliği veya değişimi uygulanır." },
            new Islem { Id = 4, Ad = "Genel Bakım", Kategori = "Telefon", MinimumFiyat = 600m, MaksimumFiyat = 1500m, Fiyat = 1050m, Aciklama = "İç temizlik, bağlantı kontrolleri ve genel performans taraması içeren temel servis bakımı." }
        );
    }
}
