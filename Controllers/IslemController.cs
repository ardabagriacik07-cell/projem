using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[AdminOnly]
public class IslemController : Controller
{
    private readonly AppDbContext _db;

    public IslemController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, string? kategori)
    {
        var query = _db.Islemler.AsQueryable();

        if (string.IsNullOrWhiteSpace(q) == false)
        {
            q = q.Trim();
            query = query.Where(x => x.Ad.Contains(q) || (x.Aciklama != null && x.Aciklama.Contains(q)));
        }

        if (string.IsNullOrWhiteSpace(kategori) == false)
        {
            kategori = kategori.Trim();
            query = query.Where(x => x.Kategori == kategori);
        }

        ViewBag.Q = q;
        ViewBag.Kategori = kategori;
        ViewBag.Kategoriler = await _db.Islemler
            .Select(x => x.Kategori)
            .Where(x => string.IsNullOrWhiteSpace(x) == false)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        return View(await query
            .OrderBy(x => x.Kategori)
            .ThenBy(x => x.Ad)
            .ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Kategoriler = OperationCatalog.Categories;
        return View(new Islem());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Islem model)
    {
        ViewBag.Kategoriler = OperationCatalog.Categories;

        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        if (model.Fiyat <= 0 && model.MinimumFiyat > 0 && model.MaksimumFiyat > 0)
        {
            model.Fiyat = (model.MinimumFiyat + model.MaksimumFiyat) / 2;
        }

        _db.Islemler.Add(model);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Yeni işlem eklendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Islemler.FindAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        ViewBag.Kategoriler = OperationCatalog.Categories;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Islem model)
    {
        ViewBag.Kategoriler = OperationCatalog.Categories;

        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var mevcut = await _db.Islemler.FindAsync(model.Id);

        if (mevcut == null)
        {
            return NotFound();
        }

        if (model.Fiyat <= 0 && model.MinimumFiyat > 0 && model.MaksimumFiyat > 0)
        {
            model.Fiyat = (model.MinimumFiyat + model.MaksimumFiyat) / 2;
        }

        mevcut.Ad = model.Ad;
        mevcut.Kategori = model.Kategori;
        mevcut.Fiyat = model.Fiyat;
        mevcut.MinimumFiyat = model.MinimumFiyat;
        mevcut.MaksimumFiyat = model.MaksimumFiyat;
        mevcut.Aciklama = model.Aciklama;

        await _db.SaveChangesAsync();

        TempData["Ok"] = "İşlem güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var kullaniliyor = await _db.ServisIslemler.AnyAsync(x => x.IslemId == id);

        if (kullaniliyor)
        {
            TempData["Hata"] = "Bu işlem servis kayıtlarında kullanıldığı için silinemedi.";
            return RedirectToAction(nameof(Index));
        }

        var model = await _db.Islemler.FindAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        _db.Islemler.Remove(model);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "İşlem silindi.";
        return RedirectToAction(nameof(Index));
    }
}
