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

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.Islemler.AsQueryable();

        if (string.IsNullOrWhiteSpace(q) == false)
        {
            q = q.Trim();
            query = query.Where(x => x.Ad.Contains(q));
        }

        ViewBag.Q = q;
        return View(await query.OrderBy(x => x.Ad).ToListAsync());
    }

    public IActionResult Create()
    {
        return View(new Islem());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Islem model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        _db.Islemler.Add(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Yeni islem eklendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _db.Islemler.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Islem model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var mevcut = await _db.Islemler.FindAsync(model.Id);
        if (mevcut == null)
        {
            return NotFound();
        }

        mevcut.Ad = model.Ad;
        mevcut.Fiyat = model.Fiyat;

        await _db.SaveChangesAsync();
        TempData["Ok"] = "Islem guncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var kullaniliyor = await _db.ServisIslemler.AnyAsync(x => x.IslemId == id);
        if (kullaniliyor)
        {
            TempData["Hata"] = "Bu islem servis kayitlarinda kullanildigi icin silinemedi.";
            return RedirectToAction(nameof(Index));
        }

        var model = await _db.Islemler.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        _db.Islemler.Remove(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Islem silindi.";
        return RedirectToAction(nameof(Index));
    }
}
