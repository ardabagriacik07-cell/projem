using System.ComponentModel.DataAnnotations;

public class UyeProfilViewModel
{
    [Required]
    [Display(Name = "Ad Soyad")]
    [StringLength(120)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Telefon")]
    [StringLength(30)]
    public string Telefon { get; set; } = string.Empty;

    [Required]
    [Display(Name = "E-posta")]
    [EmailAddress]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Yeni Şifre")]
    public string YeniSifre { get; set; } = string.Empty;
    [Display(Name = "Yeni Şifre Tekrarı")]
    public string YeniSifreTekrar { get; set; } = string.Empty;
}
