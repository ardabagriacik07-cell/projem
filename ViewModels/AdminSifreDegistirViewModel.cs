using System.ComponentModel.DataAnnotations;

public class AdminSifreDegistirViewModel
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanici adi 3-50 karakter arasinda olmali.")]
    public string KullaniciAdi { get; set; } = string.Empty;

    [Required]
    public string MevcutSifre { get; set; } = string.Empty;

    public string? YeniSifre { get; set; }

    public string? YeniSifreTekrar { get; set; }
}
