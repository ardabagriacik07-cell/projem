using Microsoft.EntityFrameworkCore;

public static class ServisBildirimHelper
{
    public const string TeslimBildirimTuru = "ServisTamamlandi";

    public static async Task TamamlananServisBildiriminiOlusturAsync(AppDbContext db, int servisId)
    {
        var servis = await db.ServisKayitlari
            .Include(x => x.Cihaz)
            .ThenInclude(x => x!.Musteri)
            .FirstOrDefaultAsync(x => x.Id == servisId);

        if (servis?.Cihaz?.Musteri == null || servis.Durum != "Tamamlandi")
        {
            return;
        }

        var bildirimVar = await db.MusteriBildirimleri.AnyAsync(x =>
            x.ServisKaydiId == servis.Id && x.Tur == TeslimBildirimTuru);

        if (bildirimVar)
        {
            return;
        }

        var cihazAdi = $"{servis.Cihaz.Marka} {servis.Cihaz.Model}".Trim();
        db.MusteriBildirimleri.Add(new MusteriBildirim
        {
            MusteriId = servis.Cihaz.MusteriId,
            ServisKaydiId = servis.Id,
            Tur = TeslimBildirimTuru,
            Baslik = "Cihazin teslim almaya hazir",
            Mesaj = $"{cihazAdi} cihazinin servis islemi tamamlandi. Musait oldugunda servis noktasina gelip urununu teslim alabilirsin.",
            OlusturmaTarihi = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }
}
