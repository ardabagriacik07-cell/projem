using System.ComponentModel.DataAnnotations;

public class UyeKayitViewModel
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

    [Required]
    [Display(Name = "Şifre")]
    [MinLength(5)]
    public string Sifre { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Şifre Tekrarı")]
    [Compare(nameof(Sifre), ErrorMessage = "Şifreler aynı olmalı.")]
    public string SifreTekrar { get; set; } = string.Empty;
}
