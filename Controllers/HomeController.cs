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
        var serviceDetails = await _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .ThenInclude(x => x!.Musteri)
            .OrderByDescending(x => x.Tarih)
            .Take(8)
            .ToListAsync();

        var tumDurumlar = await _db.ServisKayitlari.ToListAsync();
        var tumCihazlar = await _db.Cihazlar
            .Include(x => x.ServisKayitlari)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            ToplamMusteri = await _db.Musteriler.CountAsync(),
            ToplamCihaz = await _db.Cihazlar.CountAsync(),
            AktifServis = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Bekliyor" || x.Durum == "Islemde"),
            TeslimEdilen = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Teslim Edildi"),
            BekleyenServis = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Bekliyor"),
            IslemdeServis = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Islemde"),
            YeniUyelerBuAy = await _db.Musteriler.CountAsync(x => x.KayitTarihi >= monthStart),
            UyeHesabiSayisi = await _db.Musteriler.CountAsync(x => x.UyeHesabiVar),
            BuAyCiro = await _db.ServisKayitlari
                .Where(x => x.Tarih >= monthStart && (x.Durum == "Tamamlandi" || x.Durum == "Teslim Edildi"))
                .SumAsync(x => (decimal?)x.ToplamFiyat) ?? 0,
            OrtalamaServisTutari = await _db.ServisKayitlari.AnyAsync()
                ? await _db.ServisKayitlari.AverageAsync(x => x.ToplamFiyat)
                : 0,
            SonServisler = serviceDetails,
            SonUyeler = await _db.Musteriler
                .OrderByDescending(x => x.KayitTarihi)
                .Take(5)
                .ToListAsync(),
            DurumOzeti = BuildStatusItems(tumDurumlar),
            MarkaOzetleri = tumCihazlar
                .GroupBy(x => x.Marka)
                .Select(g => new AdminBrandInsightViewModel
                {
                    Marka = g.Key,
                    CihazSayisi = g.Count(),
                    ServisSayisi = g.SelectMany(x => x.ServisKayitlari).Count(),
                    ToplamCiro = g.SelectMany(x => x.ServisKayitlari).Sum(s => s.ToplamFiyat)
                })
                .OrderByDescending(x => x.ServisSayisi)
                .ThenByDescending(x => x.ToplamCiro)
                .Take(5)
                .ToList()
        };

        return View(model);
    }

    [AdminOnly]
    public async Task<IActionResult> OperasyonMerkezi()
    {
        var model = new AdminOperationsViewModel
        {
            BekleyenServisSayisi = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Bekliyor"),
            IslemdeServisSayisi = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Islemde"),
            TamamlanmisServisSayisi = await _db.ServisKayitlari.CountAsync(x => x.Durum == "Tamamlandi" || x.Durum == "Teslim Edildi"),
            BekleyenServisler = await GetServiceQuery()
                .Where(x => x.Durum == "Bekliyor")
                .OrderBy(x => x.Tarih)
                .Take(6)
                .ToListAsync(),
            IslemdeServisler = await GetServiceQuery()
                .Where(x => x.Durum == "Islemde")
                .OrderBy(x => x.Tarih)
                .Take(6)
                .ToListAsync(),
            TamamlananServisler = await GetServiceQuery()
                .Where(x => x.Durum == "Tamamlandi" || x.Durum == "Teslim Edildi")
                .OrderByDescending(x => x.Tarih)
                .Take(6)
                .ToListAsync(),
            SonEklenenCihazlar = await _db.Cihazlar
                .Include(x => x.Musteri)
                .OrderByDescending(x => x.Id)
                .Take(6)
                .ToListAsync(),
            SonUyeler = await _db.Musteriler
                .OrderByDescending(x => x.KayitTarihi)
                .Take(6)
                .ToListAsync()
        };

        return View(model);
    }

    [AdminOnly]
    public async Task<IActionResult> Raporlar()
    {
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);
        var tumServisler = await GetServiceQuery().ToListAsync();
        var tumCihazlar = await _db.Cihazlar.Include(x => x.Musteri).Include(x => x.ServisKayitlari).ToListAsync();

        var model = new AdminReportsViewModel
        {
            BuAyCiro = tumServisler
                .Where(x => x.Tarih >= thisMonthStart && (x.Durum == "Tamamlandi" || x.Durum == "Teslim Edildi"))
                .Sum(x => x.ToplamFiyat),
            GecenAyCiro = tumServisler
                .Where(x => x.Tarih >= lastMonthStart && x.Tarih < thisMonthStart && (x.Durum == "Tamamlandi" || x.Durum == "Teslim Edildi"))
                .Sum(x => x.ToplamFiyat),
            BuAyServisSayisi = tumServisler.Count(x => x.Tarih >= thisMonthStart),
            TamamlananServisSayisi = tumServisler.Count(x => x.Durum == "Tamamlandi" || x.Durum == "Teslim Edildi"),
            Son6AyCiro = BuildMonthlyRevenue(tumServisler, now),
            DurumDagilimi = BuildStatusItems(tumServisler),
            MarkaPerformansi = tumCihazlar
                .GroupBy(x => x.Marka)
                .Select(g => new AdminBrandInsightViewModel
                {
                    Marka = g.Key,
                    CihazSayisi = g.Count(),
                    ServisSayisi = g.SelectMany(x => x.ServisKayitlari).Count(),
                    ToplamCiro = g.SelectMany(x => x.ServisKayitlari).Sum(s => s.ToplamFiyat)
                })
                .OrderByDescending(x => x.ToplamCiro)
                .Take(8)
                .ToList(),
            EnDegerliMusteriler = tumCihazlar
                .Where(x => x.Musteri != null)
                .GroupBy(x => new { x.MusteriId, x.Musteri!.AdSoyad, x.Musteri.Telefon, x.Musteri.UyeHesabiVar })
                .Select(g => new AdminCustomerValueViewModel
                {
                    AdSoyad = g.Key.AdSoyad,
                    Telefon = g.Key.Telefon,
                    UyeHesabiVar = g.Key.UyeHesabiVar,
                    ServisSayisi = g.SelectMany(x => x.ServisKayitlari).Count(),
                    ToplamCiro = g.SelectMany(x => x.ServisKayitlari).Sum(s => s.ToplamFiyat)
                })
                .OrderByDescending(x => x.ToplamCiro)
                .ThenByDescending(x => x.ServisSayisi)
                .Take(8)
                .ToList()
        };

        return View(model);
    }

    [AdminOnly]
    public async Task<IActionResult> SistemDurumu()
    {
        var todayStart = DateTime.UtcNow.Date;
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        var model = new AdminSystemViewModel
        {
            ToplamAdmin = await _db.Adminler.CountAsync(),
            ToplamUye = await _db.Musteriler.CountAsync(x => x.UyeHesabiVar),
            Son7GunGirisYapanUye = await _db.Musteriler.CountAsync(x => x.SonGirisTarihi.HasValue && x.SonGirisTarihi >= sevenDaysAgo),
            BugunAcilanServis = await _db.ServisKayitlari.CountAsync(x => x.Tarih >= todayStart),
            SonGirisYapanUyeler = await _db.Musteriler
                .Where(x => x.SonGirisTarihi.HasValue)
                .OrderByDescending(x => x.SonGirisTarihi)
                .Take(7)
                .ToListAsync(),
            KritikServisler = await GetServiceQuery()
                .Where(x => x.Durum == "Bekliyor" || x.Durum == "Islemde")
                .OrderBy(x => x.Tarih)
                .Take(7)
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

    private IQueryable<ServisKaydi> GetServiceQuery()
    {
        return _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .ThenInclude(x => x!.Musteri)
            .Include(x => x.ServisIslemler);
    }

    private static List<AdminStatusBreakdownItem> BuildStatusItems(IEnumerable<ServisKaydi> servisler)
    {
        return new List<AdminStatusBreakdownItem>
        {
            new() { Label = "Bekleyen", Durum = "Bekliyor", Count = servisler.Count(x => x.Durum == "Bekliyor"), AccentClass = "warning" },
            new() { Label = "İşlemde", Durum = "Islemde", Count = servisler.Count(x => x.Durum == "Islemde"), AccentClass = "info" },
            new() { Label = "Tamamlandı", Durum = "Tamamlandi", Count = servisler.Count(x => x.Durum == "Tamamlandi"), AccentClass = "success" },
            new() { Label = "Teslim", Durum = "Teslim Edildi", Count = servisler.Count(x => x.Durum == "Teslim Edildi"), AccentClass = "purple" }
        };
    }

    private static List<AdminMonthlyRevenuePointViewModel> BuildMonthlyRevenue(IEnumerable<ServisKaydi> servisler, DateTime now)
    {
        var points = new List<AdminMonthlyRevenuePointViewModel>();

        for (var i = 5; i >= 0; i--)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1);
            var monthServices = servisler.Where(x => x.Tarih >= monthStart && x.Tarih < monthEnd).ToList();

            points.Add(new AdminMonthlyRevenuePointViewModel
            {
                Label = monthStart.ToLocalTime().ToString("MMM"),
                Ciro = monthServices
                    .Where(x => x.Durum == "Tamamlandi" || x.Durum == "Teslim Edildi")
                    .Sum(x => x.ToplamFiyat),
                ServisSayisi = monthServices.Count
            });
        }

        return points;
    }
}
