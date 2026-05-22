using System.ComponentModel.DataAnnotations;

public class UyeYorum
{
    public int Id { get; set; }
    public int MusteriId { get; set; }

    [Required]
    [StringLength(120)]
    public string Baslik { get; set; } = string.Empty;

    [Required]
    [StringLength(800)]
    public string Mesaj { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Puan { get; set; } = 5;

    public bool YoneticiGordu { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    public Musteri? Musteri { get; set; }
}
