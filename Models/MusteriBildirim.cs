using System.ComponentModel.DataAnnotations;

public class MusteriBildirim
{
    public int Id { get; set; }
    public int MusteriId { get; set; }
    public int? ServisKaydiId { get; set; }

    [Required]
    [StringLength(50)]
    public string Tur { get; set; } = "Genel";

    [Required]
    [StringLength(120)]
    public string Baslik { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Mesaj { get; set; } = string.Empty;

    public bool Okundu { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? OkunmaTarihi { get; set; }

    public Musteri? Musteri { get; set; }
    public ServisKaydi? ServisKaydi { get; set; }
}
