public class HomeDashboardViewModel
{
    public int ToplamMusteri { get; set; }
    public int ToplamCihaz { get; set; }
    public int AktifServis { get; set; }
    public int TeslimEdilen { get; set; }
    public decimal BuAyCiro { get; set; }
    public List<ServisKaydi> SonServisler { get; set; } = new();
}
