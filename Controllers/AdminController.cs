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
    public async Task<IActionResult> SifreDegistir()
    {
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

        return View(new AdminSifreDegistirViewModel
        {
            KullaniciAdi = admin.KullaniciAdi
        });
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

        var yeniKullaniciAdi = model.KullaniciAdi.Trim();
        if (string.IsNullOrWhiteSpace(yeniKullaniciAdi))
        {
            ModelState.AddModelError(nameof(model.KullaniciAdi), "Kullanici adi bos olamaz.");
            return View(model);
        }

        if (yeniKullaniciAdi.Length < 3 || yeniKullaniciAdi.Length > 50)
        {
            ModelState.AddModelError(nameof(model.KullaniciAdi), "Kullanici adi 3-50 karakter arasinda olmali.");
            return View(model);
        }

        var kullaniciAdiVar = await _db.Adminler.AnyAsync(x =>
            x.Id != admin.Id && x.KullaniciAdi.ToLower() == yeniKullaniciAdi.ToLower());

        if (kullaniciAdiVar)
        {
            ModelState.AddModelError(nameof(model.KullaniciAdi), "Bu kullanici adi baska bir admin tarafindan kullaniliyor.");
            return View(model);
        }

        var sifreDegistirilecek = string.IsNullOrWhiteSpace(model.YeniSifre) == false ||
            string.IsNullOrWhiteSpace(model.YeniSifreTekrar) == false;

        if (sifreDegistirilecek)
        {
            if (string.IsNullOrWhiteSpace(model.YeniSifre) || model.YeniSifre.Length < 5)
            {
                ModelState.AddModelError(nameof(model.YeniSifre), "Yeni sifre en az 5 karakter olmali.");
                return View(model);
            }

            if (model.YeniSifre != model.YeniSifreTekrar)
            {
                ModelState.AddModelError(nameof(model.YeniSifreTekrar), "Yeni sifreler eslesmiyor.");
                return View(model);
            }

            admin.Sifre = model.YeniSifre;
        }

        admin.KullaniciAdi = yeniKullaniciAdi;
        await _db.SaveChangesAsync();
        HttpContext.Session.SetString("Admin", admin.KullaniciAdi);
        TempData["Ok"] = sifreDegistirilecek
            ? "Admin kullanici adi ve sifresi guncellendi."
            : "Admin kullanici adi guncellendi.";
        return RedirectToAction(nameof(SifreDegistir));
    }
}
