using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UyeController : Controller
{
    private readonly AppDbContext _db;

    public UyeController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Giris()
    {
        if (HttpContext.Session.GetInt32("UyeId").HasValue)
        {
            return RedirectToAction(nameof(Panel));
        }

        return View(new UyeGirisViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Giris(UyeGirisViewModel model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();

        var uye = await _db.Musteriler
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.UyeHesabiVar && x.Sifre == model.Sifre);

        if (uye == null)
        {
            ModelState.AddModelError(string.Empty, "Email veya sifre hatali.");
            return View(model);
        }

        uye.SonGirisTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        HttpContext.Session.Remove("Admin");
        HttpContext.Session.SetInt32("UyeId", uye.Id);
        HttpContext.Session.SetString("UyeAd", uye.AdSoyad);
        return RedirectToAction(nameof(Panel));
    }

    public IActionResult Kayit()
    {
        if (HttpContext.Session.GetInt32("UyeId").HasValue)
        {
            return RedirectToAction(nameof(Panel));
        }

        return View(new UyeKayitViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kayit(UyeKayitViewModel model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();
        var emailVar = await _db.Musteriler.AnyAsync(x => x.Email.ToLower() == email);

        if (emailVar)
        {
            ModelState.AddModelError(nameof(model.Email), "Bu email zaten kullaniliyor.");
            return View(model);
        }

        var uye = new Musteri
        {
            AdSoyad = model.AdSoyad.Trim(),
            Telefon = model.Telefon.Trim(),
            Email = email,
            Sifre = model.Sifre,
            UyeHesabiVar = true,
            KayitTarihi = DateTime.UtcNow
        };

        _db.Musteriler.Add(uye);
        await _db.SaveChangesAsync();

        HttpContext.Session.Remove("Admin");
        HttpContext.Session.SetInt32("UyeId", uye.Id);
        HttpContext.Session.SetString("UyeAd", uye.AdSoyad);
        TempData["UyeOk"] = "Kayit basarili. Hos geldin.";
        return RedirectToAction(nameof(Panel));
    }

    [UyeOnly]
    public async Task<IActionResult> Panel()
    {
        var uyeId = HttpContext.Session.GetInt32("UyeId");
        if (uyeId.HasValue == false)
        {
            return RedirectToAction(nameof(Giris));
        }

        var uye = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == uyeId.Value);
        if (uye == null)
        {
            return RedirectToAction(nameof(Cikis));
        }

        var servisler = await _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .Where(x => x.Cihaz != null && x.Cihaz.MusteriId == uyeId.Value)
            .OrderByDescending(x => x.Tarih)
            .ToListAsync();

        var model = new UyePanelViewModel
        {
            Uye = uye,
            CihazSayisi = await _db.Cihazlar.CountAsync(x => x.MusteriId == uyeId.Value),
            AktifTalep = servisler.Count(x => x.Durum == "Bekliyor" || x.Durum == "Islemde"),
            ToplamTalep = servisler.Count,
            SonTalepler = servisler.Take(6).ToList()
        };

        return View(model);
    }

    [UyeOnly]
    public async Task<IActionResult> Taleplerim()
    {
        var uyeId = HttpContext.Session.GetInt32("UyeId");
        if (uyeId.HasValue == false)
        {
            return RedirectToAction(nameof(Giris));
        }

        var talepler = await _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .ThenInclude(x => x!.Musteri)
            .Include(x => x.ServisIslemler)
            .ThenInclude(x => x.Islem)
            .Where(x => x.Cihaz != null && x.Cihaz.MusteriId == uyeId.Value)
            .OrderByDescending(x => x.Tarih)
            .ToListAsync();

        return View(talepler);
    }

    [UyeOnly]
    public IActionResult YeniTalep()
    {
        return View(new UyeTalepCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [UyeOnly]
    public async Task<IActionResult> YeniTalep(UyeTalepCreateViewModel form)
    {
        var uyeId = HttpContext.Session.GetInt32("UyeId");
        if (uyeId.HasValue == false)
        {
            return RedirectToAction(nameof(Giris));
        }

        if (ModelState.IsValid == false)
        {
            return View(form);
        }

        var cihaz = new Cihaz
        {
            MusteriId = uyeId.Value,
            Marka = form.Marka.Trim(),
            Model = form.Model.Trim(),
            ArizaAciklama = form.ArizaAciklama.Trim()
        };

        _db.Cihazlar.Add(cihaz);
        await _db.SaveChangesAsync();

        var servis = new ServisKaydi
        {
            CihazId = cihaz.Id,
            Tarih = DateTime.UtcNow,
            Durum = "Bekliyor",
            ToplamFiyat = 0
        };

        _db.ServisKayitlari.Add(servis);
        await _db.SaveChangesAsync();

        TempData["UyeOk"] = "Talebin alindi. En kisa surede inceleyecegiz.";
        return RedirectToAction(nameof(Taleplerim));
    }

    [UyeOnly]
    public async Task<IActionResult> Profil()
    {
        var uyeId = HttpContext.Session.GetInt32("UyeId");
        if (uyeId.HasValue == false)
        {
            return RedirectToAction(nameof(Giris));
        }

        var uye = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == uyeId.Value);
        if (uye == null)
        {
            return RedirectToAction(nameof(Cikis));
        }

        return View(new UyeProfilViewModel
        {
            AdSoyad = uye.AdSoyad,
            Telefon = uye.Telefon,
            Email = uye.Email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [UyeOnly]
    public async Task<IActionResult> Profil(UyeProfilViewModel model)
    {
        var uyeId = HttpContext.Session.GetInt32("UyeId");
        if (uyeId.HasValue == false)
        {
            return RedirectToAction(nameof(Giris));
        }

        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var uye = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == uyeId.Value);
        if (uye == null)
        {
            return RedirectToAction(nameof(Cikis));
        }

        var yeniEmail = model.Email.Trim().ToLowerInvariant();
        var emailVar = await _db.Musteriler.AnyAsync(x => x.Id != uye.Id && x.Email.ToLower() == yeniEmail);

        if (emailVar)
        {
            ModelState.AddModelError(nameof(model.Email), "Bu email baska bir hesapta kullaniliyor.");
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.YeniSifre) == false || string.IsNullOrWhiteSpace(model.YeniSifreTekrar) == false)
        {
            if (model.YeniSifre != model.YeniSifreTekrar)
            {
                ModelState.AddModelError(nameof(model.YeniSifreTekrar), "Yeni sifreler eslesmiyor.");
                return View(model);
            }

            if (model.YeniSifre.Length < 5)
            {
                ModelState.AddModelError(nameof(model.YeniSifre), "Sifre en az 5 karakter olmali.");
                return View(model);
            }

            uye.Sifre = model.YeniSifre;
        }

        uye.AdSoyad = model.AdSoyad.Trim();
        uye.Telefon = model.Telefon.Trim();
        uye.Email = yeniEmail;

        await _db.SaveChangesAsync();

        HttpContext.Session.SetString("UyeAd", uye.AdSoyad);
        TempData["UyeOk"] = "Profilin guncellendi.";
        return RedirectToAction(nameof(Profil));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cikis()
    {
        HttpContext.Session.Remove("UyeId");
        HttpContext.Session.Remove("UyeAd");
        return RedirectToAction(nameof(Giris));
    }
}
