using System.ComponentModel.DataAnnotations;

public class UyeSifreSifirlaViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Kod 6 haneli olmalidir.")]
    public string Kod { get; set; } = string.Empty;

    [Required]
    [MinLength(5, ErrorMessage = "Sifre en az 5 karakter olmali.")]
    public string YeniSifre { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(YeniSifre), ErrorMessage = "Sifreler eslesmiyor.")]
    public string YeniSifreTekrar { get; set; } = string.Empty;
}
