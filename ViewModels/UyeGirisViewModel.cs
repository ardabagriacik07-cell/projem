using System.ComponentModel.DataAnnotations;

public class UyeGirisViewModel
{
    [Required]
    [Display(Name = "E-posta")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Şifre")]
    public string Sifre { get; set; } = string.Empty;
}
