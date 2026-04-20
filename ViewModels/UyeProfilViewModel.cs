using System.ComponentModel.DataAnnotations;

public class UyeProfilViewModel
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

    public string YeniSifre { get; set; } = string.Empty;
    public string YeniSifreTekrar { get; set; } = string.Empty;
}
