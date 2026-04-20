using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projem.Models;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    [AdminOnly]
    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var model = new HomeDashboardViewModel
        {
            ToplamMusteri = await _db.Musteriler.CountAsync(),
            ToplamCihaz = await _db.Cihazlar.CountAsync(),
            AktifServis = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Bekliyor" || x.Durum == "Islemde"),
            TeslimEdilen = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Teslim Edildi"),
            BuAyCiro = await _db.ServisKayitlari
                .Where(x => x.Tarih >= monthStart && (x.Durum == "Tamamlandi" || x.Durum == "Teslim Edildi"))
                .SumAsync(x => (decimal?)x.ToplamFiyat) ?? 0,
            SonServisler = await _db.ServisKayitlari
                .Include(x => x.Cihaz)
                .ThenInclude(x => x!.Musteri)
                .OrderByDescending(x => x.Tarih)
                .Take(6)
                .ToListAsync()
        };

        return View(model);
    }

    [AdminOnly]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
