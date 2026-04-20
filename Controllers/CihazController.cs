using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[AdminOnly]
public class CihazController : Controller
{
    private readonly AppDbContext _db;

    public CihazController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, int? musteriId)
    {
        var query = _db.Cihazlar
            .Include(x => x.Musteri)
            .Include(x => x.ServisKayitlari)
            .AsQueryable();

        if (string.IsNullOrWhiteSpace(q) == false)
        {
            q = q.Trim();
            query = query.Where(x =>
                x.Marka.Contains(q) ||
                x.Model.Contains(q) ||
                x.ArizaAciklama.Contains(q) ||
                (x.Musteri != null && x.Musteri.AdSoyad.Contains(q)));
        }

        if (musteriId.HasValue)
        {
            query = query.Where(x => x.MusteriId == musteriId.Value);
        }

        ViewBag.Musteriler = await _db.Musteriler
            .OrderBy(x => x.AdSoyad)
            .Select(x => new SelectListItem(x.AdSoyad, x.Id.ToString()))
            .ToListAsync();

        ViewBag.Q = q;
        ViewBag.MusteriId = musteriId;

        var cihazlar = await query.OrderByDescending(x => x.Id).ToListAsync();
        return View(cihazlar);
    }

    public async Task<IActionResult> Details(int id)
    {
        var cihaz = await _db.Cihazlar
            .Include(x => x.Musteri)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cihaz == null)
        {
            return NotFound();
        }

        var servisler = await _db.ServisKayitlari
            .Where(x => x.CihazId == id)
            .Include(x => x.ServisIslemler)
            .OrderByDescending(x => x.Tarih)
            .ToListAsync();

        return View(new CihazDetailsViewModel
        {
            Cihaz = cihaz,
            ServisKayitlari = servisler
        });
    }

    public async Task<IActionResult> Create()
    {
        await HazirlaMusteriler();
        return View(new Cihaz());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cihaz form)
    {
        if (ModelState.IsValid == false)
        {
            await HazirlaMusteriler();
            return View(form);
        }

        _db.Cihazlar.Add(form);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Cihaz kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Cihazlar.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        await HazirlaMusteriler();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Cihaz form)
    {
        if (ModelState.IsValid == false)
        {
            await HazirlaMusteriler();
            return View(form);
        }

        var mevcut = await _db.Cihazlar.FindAsync(form.Id);
        if (mevcut == null)
        {
            return NotFound();
        }

        mevcut.MusteriId = form.MusteriId;
        mevcut.Marka = form.Marka;
        mevcut.Model = form.Model;
        mevcut.ArizaAciklama = form.ArizaAciklama;

        await _db.SaveChangesAsync();
        TempData["Ok"] = "Cihaz kaydi guncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _db.Cihazlar.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        _db.Cihazlar.Remove(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Cihaz silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task HazirlaMusteriler()
    {
        ViewBag.Musteriler = await _db.Musteriler
            .OrderBy(x => x.AdSoyad)
            .Select(x => new SelectListItem(x.AdSoyad, x.Id.ToString()))
            .ToListAsync();
    }
}
