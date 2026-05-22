using System.ComponentModel.DataAnnotations;

public class AdminSifreDegistirViewModel
{
    [Required]
    [Display(Name = "Kullanıcı Adı")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalı.")]
    public string KullaniciAdi { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Mevcut Şifre")]
    public string MevcutSifre { get; set; } = string.Empty;

    [Display(Name = "Yeni Şifre")]
    public string? YeniSifre { get; set; }

    [Display(Name = "Yeni Şifre Tekrarı")]
    public string? YeniSifreTekrar { get; set; }
}
