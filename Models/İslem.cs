using System.ComponentModel.DataAnnotations;

public class Islem
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "İşlem Adı")]
    [StringLength(120)]
    public string Ad { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Kategori")]
    [StringLength(50)]
    public string Kategori { get; set; } = "Genel";

    [Range(0, 999999)]
    [Display(Name = "Ortalama Fiyat")]
    public decimal Fiyat { get; set; }

    [Range(0, 999999)]
    [Display(Name = "Minimum Fiyat")]
    public decimal MinimumFiyat { get; set; }

    [Range(0, 999999)]
    [Display(Name = "Maksimum Fiyat")]
    public decimal MaksimumFiyat { get; set; }

    [Display(Name = "Açıklama")]
    [StringLength(300)]
    public string? Aciklama { get; set; }

    public List<ServisIslem> ServisIslemler { get; set; } = new();
}
