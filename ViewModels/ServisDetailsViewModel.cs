public class ServisDetailsViewModel
{
    public ServisKaydi Servis { get; set; } = new();
    public List<Islem> TumIslemler { get; set; } = new();
    public List<int> SeciliIslemIdleri { get; set; } = new();
}
