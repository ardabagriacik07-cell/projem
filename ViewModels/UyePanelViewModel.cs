public class UyePanelViewModel
{
    public Musteri Uye { get; set; } = new();
    public int CihazSayisi { get; set; }
    public int AktifTalep { get; set; }
    public int ToplamTalep { get; set; }
    public int OkunmamisBildirimSayisi { get; set; }
    public List<ServisKaydi> SonTalepler { get; set; } = new();
    public List<MusteriBildirim> SonBildirimler { get; set; } = new();
    public UyeYorumCreateViewModel YorumForm { get; set; } = new();
    public List<UyeYorum> SonYorumlar { get; set; } = new();
}
