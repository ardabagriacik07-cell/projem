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

        modelBuilder.Entity<Cihaz>()
            .HasMany(x => x.ServisKayitlari)
            .WithOne(x => x.Cihaz)
            .HasForeignKey(x => x.CihazId)
            .OnDelete(DeleteBehavior.Cascade);

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
            new Islem { Id = 1, Ad = "Ekran Degisimi", Fiyat = 2450m },
            new Islem { Id = 2, Ad = "Batarya Degisimi", Fiyat = 1250m },
            new Islem { Id = 3, Ad = "Soket Tamiri", Fiyat = 900m },
            new Islem { Id = 4, Ad = "Genel Bakim", Fiyat = 650m }
        );
    }
}
