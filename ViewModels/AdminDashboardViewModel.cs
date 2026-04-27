public class AdminDashboardViewModel
{
    public int ToplamMusteri { get; set; }
    public int ToplamCihaz { get; set; }
    public int AktifServis { get; set; }
    public int TeslimEdilen { get; set; }
    public int BekleyenServis { get; set; }
    public int IslemdeServis { get; set; }
    public int YeniUyelerBuAy { get; set; }
    public int UyeHesabiSayisi { get; set; }
    public decimal BuAyCiro { get; set; }
    public decimal OrtalamaServisTutari { get; set; }
    public List<ServisKaydi> SonServisler { get; set; } = new();
    public List<Musteri> SonUyeler { get; set; } = new();
    public List<AdminStatusBreakdownItem> DurumOzeti { get; set; } = new();
    public List<AdminBrandInsightViewModel> MarkaOzetleri { get; set; } = new();
}
