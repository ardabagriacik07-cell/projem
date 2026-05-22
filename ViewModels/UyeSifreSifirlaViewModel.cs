using System.ComponentModel.DataAnnotations;

public class UyeSifreSifirlaViewModel
{
    [Required]
    [Display(Name = "E-posta")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Kod")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Kod 6 haneli olmalıdır.")]
    public string Kod { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Yeni Şifre")]
    [MinLength(5, ErrorMessage = "Şifre en az 5 karakter olmalı.")]
    public string YeniSifre { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Yeni Şifre Tekrarı")]
    [Compare(nameof(YeniSifre), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string YeniSifreTekrar { get; set; } = string.Empty;
}
