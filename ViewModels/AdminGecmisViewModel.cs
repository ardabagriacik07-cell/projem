public class AdminGecmisViewModel
{
    public DateTime? Tarih { get; set; }
    public string? Durum { get; set; }
    public string? Q { get; set; }
    public int ServisSayisi { get; set; }
    public decimal ToplamTutar { get; set; }
    public List<string> Durumlar { get; set; } = new();
    public List<ServisKaydi> Servisler { get; set; } = new();
}
