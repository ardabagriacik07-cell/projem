using System.ComponentModel.DataAnnotations;

public class UyeSifreKodGonderViewModel
{
    [Required]
    [Display(Name = "E-posta")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
