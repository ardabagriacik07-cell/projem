using System.ComponentModel.DataAnnotations;

public class Islem
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Ad { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Kategori { get; set; } = "Genel";

    [Range(0, 999999)]
    public decimal Fiyat { get; set; }

    [Range(0, 999999)]
    public decimal MinimumFiyat { get; set; }

    [Range(0, 999999)]
    public decimal MaksimumFiyat { get; set; }

    [StringLength(300)]
    public string? Aciklama { get; set; }

    public List<ServisIslem> ServisIslemler { get; set; } = new();
}