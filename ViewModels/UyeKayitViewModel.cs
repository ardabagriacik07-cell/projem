using System.ComponentModel.DataAnnotations;

public class UyeKayitViewModel
{
    [Required]
    [StringLength(120)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Telefon { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(5)]
    public string Sifre { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Sifre), ErrorMessage = "Sifreler ayni olmali.")]
    public string SifreTekrar { get; set; } = string.Empty;
}
