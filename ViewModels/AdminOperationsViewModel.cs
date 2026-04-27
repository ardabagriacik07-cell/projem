public class AdminOperationsViewModel
{
    public int BekleyenServisSayisi { get; set; }
    public int IslemdeServisSayisi { get; set; }
    public int TamamlanmisServisSayisi { get; set; }
    public List<ServisKaydi> BekleyenServisler { get; set; } = new();
    public List<ServisKaydi> IslemdeServisler { get; set; } = new();
    public List<ServisKaydi> TamamlananServisler { get; set; } = new();
    public List<Cihaz> SonEklenenCihazlar { get; set; } = new();
    public List<Musteri> SonUyeler { get; set; } = new();
}
