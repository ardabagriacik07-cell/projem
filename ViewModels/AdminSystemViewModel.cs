public class AdminSystemViewModel
{
    public int ToplamAdmin { get; set; }
    public int ToplamUye { get; set; }
    public int Son7GunGirisYapanUye { get; set; }
    public int BugunAcilanServis { get; set; }
    public List<Musteri> SonGirisYapanUyeler { get; set; } = new();
    public List<ServisKaydi> KritikServisler { get; set; } = new();
}
