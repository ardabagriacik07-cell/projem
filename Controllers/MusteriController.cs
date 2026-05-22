using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[AdminOnly]
public class MusteriController : Controller
{
    private readonly AppDbContext _db;

    public MusteriController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.Musteriler.AsQueryable();

        if (string.IsNullOrWhiteSpace(q) == false)
        {
            q = q.Trim();
            query = query.Where(x => x.AdSoyad.Contains(q) || x.Telefon.Contains(q) || x.Email.Contains(q));
        }

        var musteriler = await query
            .Include(x => x.Cihazlar)
            .OrderBy(x => x.AdSoyad)
            .ToListAsync();

        ViewBag.Q = q;
        return View(musteriler);
    }

    public async Task<IActionResult> Details(int id)
    {
        var musteri = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == id);
        if (musteri == null)
        {
            return NotFound();
        }

        var cihazlar = await _db.Cihazlar
            .Where(x => x.MusteriId == id)
            .Include(x => x.ServisKayitlari)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return View(new MusteriDetailsViewModel
        {
            Musteri = musteri,
            Cihazlar = cihazlar
        });
    }

    public async Task<IActionResult> Pdf(int id)
    {
        var musteri = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == id);
        if (musteri == null)
        {
            return NotFound();
        }

        var servisler = await _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .ThenInclude(x => x!.Musteri)
            .Include(x => x.ServisIslemler)
            .ThenInclude(x => x.Islem)
            .Where(x => x.Cihaz != null && x.Cihaz.MusteriId == id)
            .OrderByDescending(x => x.Tarih)
            .ToListAsync();

        var rapor = new AdminGecmisViewModel
        {
            Q = musteri.AdSoyad,
            ServisSayisi = servisler.Count,
            ToplamTutar = servisler.Sum(x => x.ToplamFiyat),
            Servisler = servisler
        };

        var pdf = FixoriaPdfReportBuilder.BuildServiceHistoryReport(servisler, rapor);
        var safeName = string.Join("-", musteri.AdSoyad
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToLowerInvariant();

        return File(pdf, "application/pdf", $"fixoria-{safeName}-servis-gecmisi.pdf");
    }

    public IActionResult Create()
    {
        return View(new Musteri());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Musteri model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        model.UyeHesabiVar = false;
        model.Sifre = string.Empty;
        model.KayitTarihi = DateTime.UtcNow;
        _db.Musteriler.Add(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Müşteri kaydı oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Musteriler.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Musteri model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var mevcut = await _db.Musteriler.FindAsync(model.Id);
        if (mevcut == null)
        {
            return NotFound();
        }

        var yeniEmail = model.Email.Trim().ToLowerInvariant();
        var emailVar = await _db.Musteriler.AnyAsync(x => x.Id != model.Id && x.Email.ToLower() == yeniEmail);
        if (emailVar)
        {
            ModelState.AddModelError(nameof(model.Email), "Bu e-posta başka bir müşteri tarafından kullanılıyor.");
            return View(model);
        }

        mevcut.AdSoyad = model.AdSoyad.Trim();
        mevcut.Telefon = model.Telefon.Trim();
        mevcut.Email = yeniEmail;
        mevcut.Sifre = model.Sifre?.Trim() ?? string.Empty;
        mevcut.UyeHesabiVar = model.UyeHesabiVar;
        mevcut.KayitTarihi = model.KayitTarihi;
        mevcut.SonGirisTarihi = model.SonGirisTarihi;
        mevcut.SifreSifirlamaKodu = string.IsNullOrWhiteSpace(model.SifreSifirlamaKodu) ? null : model.SifreSifirlamaKodu.Trim();
        mevcut.SifreSifirlamaKodSonTarih = model.SifreSifirlamaKodSonTarih;

        await _db.SaveChangesAsync();
        TempData["Ok"] = "Müşteri bilgileri güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _db.Musteriler.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        _db.Musteriler.Remove(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Müşteri silindi.";
        return RedirectToAction(nameof(Index));
    }
}
