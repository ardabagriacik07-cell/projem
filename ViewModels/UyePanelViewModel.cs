public class UyePanelViewModel
{
    public Musteri Uye { get; set; } = new();
    public int CihazSayisi { get; set; }
    public int AktifTalep { get; set; }
    public int ToplamTalep { get; set; }
    public List<ServisKaydi> SonTalepler { get; set; } = new();
}
