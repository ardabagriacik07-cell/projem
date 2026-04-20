using System.ComponentModel.DataAnnotations;

public class AdminSifreDegistirViewModel
{
    [Required]
    public string MevcutSifre { get; set; } = string.Empty;

    [Required]
    [MinLength(5)]
    public string YeniSifre { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(YeniSifre), ErrorMessage = "Yeni sifreler eslesmiyor.")]
    public string YeniSifreTekrar { get; set; } = string.Empty;
}
