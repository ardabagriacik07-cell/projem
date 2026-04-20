using System.ComponentModel.DataAnnotations;

public class ServisKaydi
{
    public int Id { get; set; }
    public int CihazId { get; set; }

    [DataType(DataType.Date)]
    public DateTime Tarih { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(30)]
    public string Durum { get; set; } = "Bekliyor";

    [Range(0, 999999)]
    public decimal ToplamFiyat { get; set; }

    public Cihaz? Cihaz { get; set; }
    public List<ServisIslem> ServisIslemler { get; set; } = new();
}
