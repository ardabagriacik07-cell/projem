using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Login()
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Admin")) == false)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
    {
        var admin = await _db.Adminler
            .FirstOrDefaultAsync(x => x.KullaniciAdi == kullaniciAdi && x.Sifre == sifre);

        if (admin != null)
        {
            HttpContext.Session.Remove("UyeId");
            HttpContext.Session.Remove("UyeAd");
            HttpContext.Session.SetString("Admin", admin.KullaniciAdi);
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Hata = "Kullanici adi veya sifre hatali.";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [AdminOnly]
    public IActionResult SifreDegistir()
    {
        return View(new AdminSifreDegistirViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnly]
    public async Task<IActionResult> SifreDegistir(AdminSifreDegistirViewModel model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var adminAd = HttpContext.Session.GetString("Admin");
        if (string.IsNullOrWhiteSpace(adminAd))
        {
            return RedirectToAction(nameof(Login));
        }

        var admin = await _db.Adminler.FirstOrDefaultAsync(x => x.KullaniciAdi == adminAd);
        if (admin == null)
        {
            return RedirectToAction(nameof(Login));
        }

        if (admin.Sifre != model.MevcutSifre)
        {
            ModelState.AddModelError(nameof(model.MevcutSifre), "Mevcut sifre dogru degil.");
            return View(model);
        }

        admin.Sifre = model.YeniSifre;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Admin sifresi guncellendi.";
        return RedirectToAction(nameof(SifreDegistir));
    }
}
