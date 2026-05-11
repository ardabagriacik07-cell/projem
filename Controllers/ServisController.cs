using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[AdminOnly]
public class ServisController : Controller
{
    private readonly AppDbContext _db;
    private readonly IEmailSender _emailSender;

    public ServisController(AppDbContext db, IEmailSender emailSender)
    {
        _db = db;
        _emailSender = emailSender;
    }

    public async Task<IActionResult> Index(string? q, string? durum)
    {
        var query = _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .ThenInclude(x => x!.Musteri)
            .Include(x => x.ServisIslemler)
            .ThenInclude(x => x.Islem)
            .AsQueryable();

        if (string.IsNullOrWhiteSpace(q) == false)
        {
            q = q.Trim();
            query = query.Where(x =>
                (x.Cihaz != null && (x.Cihaz.Marka.Contains(q) || x.Cihaz.Model.Contains(q))) ||
                (x.Cihaz != null && x.Cihaz.Musteri != null && x.Cihaz.Musteri.AdSoyad.Contains(q)));
        }

        if (string.IsNullOrWhiteSpace(durum) == false)
        {
            query = query.Where(x => x.Durum == durum);
        }

        ViewBag.Q = q;
        ViewBag.Durum = durum;

        var list = await query.OrderByDescending(x => x.Tarih).ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var servis = await _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .ThenInclude(x => x!.Musteri)
            .Include(x => x.ServisIslemler)
            .ThenInclude(x => x.Islem)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (servis == null)
        {
            return NotFound();
        }

        return View(new ServisDetailsViewModel
        {
            Servis = servis,
            TumIslemler = await _db.Islemler.OrderBy(x => x.Ad).ToListAsync(),
            SeciliIslemIdleri = servis.ServisIslemler.Select(x => x.IslemId).ToList()
        });
    }

    public async Task<IActionResult> Create()
    {
        var vm = new ServisFormViewModel
        {
            Servis = new ServisKaydi { Tarih = DateTime.UtcNow, Durum = "Bekliyor" },
            Cihazlar = await _db.Cihazlar.Include(x => x.Musteri).OrderByDescending(x => x.Id).ToListAsync(),
            Islemler = await _db.Islemler.OrderBy(x => x.Ad).ToListAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServisFormViewModel vm)
    {
        vm.Cihazlar = await _db.Cihazlar.Include(x => x.Musteri).OrderByDescending(x => x.Id).ToListAsync();
        vm.Islemler = await _db.Islemler.OrderBy(x => x.Ad).ToListAsync();
        ModelState.Remove("Servis.FiyatOnayDurumu");
        ModelState.Remove("Servis.FiyatOnayTarihi");
        ModelState.Remove("Servis.FiyatCevapTarihi");

        if (ModelState.IsValid == false)
        {
            return View(vm);
        }

        var seciliIslemler = vm.Islemler.Where(x => vm.SeciliIslemIdleri.Contains(x.Id)).ToList();

        var servis = new ServisKaydi
        {
            CihazId = vm.Servis.CihazId,
            Tarih = vm.Servis.Tarih,
            Durum = vm.Servis.Durum,
            ToplamFiyat = seciliIslemler.Sum(x => x.Fiyat),
            FiyatOnayDurumu = "Onay Gerekmez"
        };

        if (vm.FiyatiOnayaGonder && servis.ToplamFiyat > 0)
        {
            servis.Durum = "Fiyat Onayi Bekliyor";
            servis.FiyatOnayDurumu = "Onay Bekliyor";
            servis.FiyatOnayTarihi = DateTime.UtcNow;
        }

        _db.ServisKayitlari.Add(servis);
        await _db.SaveChangesAsync();

        if (vm.SeciliIslemIdleri.Count > 0)
        {
            foreach (var islemId in vm.SeciliIslemIdleri.Distinct())
            {
                _db.ServisIslemler.Add(new ServisIslem { ServisKaydiId = servis.Id, IslemId = islemId });
            }

            await _db.SaveChangesAsync();
        }

        if (servis.FiyatOnayDurumu == "Onay Bekliyor")
        {
            await FiyatOnayMailiGonderAsync(servis.Id);
        }

        TempData["Ok"] = "Servis kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var servis = await _db.ServisKayitlari
            .Include(x => x.ServisIslemler)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (servis == null)
        {
            return NotFound();
        }

        return View(new ServisFormViewModel
        {
            Servis = servis,
            Cihazlar = await _db.Cihazlar.Include(x => x.Musteri).OrderByDescending(x => x.Id).ToListAsync(),
            Islemler = await _db.Islemler.OrderBy(x => x.Ad).ToListAsync(),
            SeciliIslemIdleri = servis.ServisIslemler.Select(x => x.IslemId).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ServisFormViewModel vm)
    {
        vm.Cihazlar = await _db.Cihazlar.Include(x => x.Musteri).OrderByDescending(x => x.Id).ToListAsync();
        vm.Islemler = await _db.Islemler.OrderBy(x => x.Ad).ToListAsync();
        ModelState.Remove("Servis.FiyatOnayDurumu");
        ModelState.Remove("Servis.FiyatOnayTarihi");
        ModelState.Remove("Servis.FiyatCevapTarihi");

        if (ModelState.IsValid == false)
        {
            return View(vm);
        }

        var servis = await _db.ServisKayitlari
            .Include(x => x.ServisIslemler)
            .FirstOrDefaultAsync(x => x.Id == vm.Servis.Id);

        if (servis == null)
        {
            return NotFound();
        }

        servis.CihazId = vm.Servis.CihazId;
        servis.Tarih = vm.Servis.Tarih;
        servis.Durum = vm.Servis.Durum;
        if (vm.FiyatiOnayaGonder == false && servis.FiyatOnayDurumu == "Onay Bekliyor")
        {
            servis.FiyatOnayDurumu = "Onay Gerekmez";
            servis.FiyatOnayTarihi = null;
            servis.FiyatCevapTarihi = null;
        }

        var mevcutIslemKayitlari = servis.ServisIslemler.ToList();
        if (mevcutIslemKayitlari.Count > 0)
        {
            _db.ServisIslemler.RemoveRange(mevcutIslemKayitlari);
        }

        foreach (var islemId in vm.SeciliIslemIdleri.Distinct())
        {
            _db.ServisIslemler.Add(new ServisIslem { ServisKaydiId = servis.Id, IslemId = islemId });
        }

        var seciliIslemTutari = await _db.Islemler
            .Where(x => vm.SeciliIslemIdleri.Contains(x.Id))
            .SumAsync(x => (decimal?)x.Fiyat) ?? 0;

        servis.ToplamFiyat = seciliIslemTutari;

        await _db.SaveChangesAsync();

        if (vm.FiyatiOnayaGonder && servis.ToplamFiyat > 0)
        {
            servis.Durum = "Fiyat Onayi Bekliyor";
            servis.FiyatOnayDurumu = "Onay Bekliyor";
            servis.FiyatOnayTarihi = DateTime.UtcNow;
            servis.FiyatCevapTarihi = null;
            await _db.SaveChangesAsync();
            await FiyatOnayMailiGonderAsync(servis.Id);
        }

        TempData["Ok"] = "Servis kaydi guncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var servis = await _db.ServisKayitlari.FindAsync(id);
        if (servis == null)
        {
            return NotFound();
        }

        _db.ServisKayitlari.Remove(servis);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Servis kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task FiyatOnayMailiGonderAsync(int servisId)
    {
        var servis = await _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .ThenInclude(x => x!.Musteri)
            .FirstOrDefaultAsync(x => x.Id == servisId);

        if (servis?.Cihaz?.Musteri == null || string.IsNullOrWhiteSpace(servis.Cihaz.Musteri.Email))
        {
            return;
        }

        var body = $"""
            <div style="font-family:Arial,sans-serif;font-size:16px;color:#111827">
                <p>Merhaba {servis.Cihaz.Musteri.AdSoyad},</p>
                <p>{servis.Cihaz.Marka} {servis.Cihaz.Model} cihazın için servis fiyatı hazırlandı.</p>
                <p style="font-size:28px;font-weight:700;margin:16px 0;">{servis.ToplamFiyat:C0}</p>
                <p>Üye panelindeki Taleplerim ekranından fiyatı kabul veya reddedebilirsin.</p>
            </div>
            """;

        try
        {
            await _emailSender.SendAsync(servis.Cihaz.Musteri.Email, "Servis Plus Fiyat Onayi", body);
        }
        catch
        {
            TempData["Hata"] = "Fiyat onay maili gonderilemedi. Servis kaydi yine de kaydedildi.";
        }
    }
}
