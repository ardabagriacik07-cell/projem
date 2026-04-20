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
        TempData["Ok"] = "Musteri kaydi olusturuldu.";
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

        mevcut.AdSoyad = model.AdSoyad;
        mevcut.Telefon = model.Telefon;
        mevcut.Email = model.Email;

        await _db.SaveChangesAsync();
        TempData["Ok"] = "Musteri bilgileri guncellendi.";
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
        TempData["Ok"] = "Musteri silindi.";
        return RedirectToAction(nameof(Index));
    }
}
