public class ServisFormViewModel
{
    public ServisKaydi Servis { get; set; } = new();
    public List<Cihaz> Cihazlar { get; set; } = new();
    public List<Islem> Islemler { get; set; } = new();
    public List<int> SeciliIslemIdleri { get; set; } = new();
}
