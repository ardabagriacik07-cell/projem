using System.ComponentModel.DataAnnotations;

public class UyeGirisViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Sifre { get; set; } = string.Empty;
}
