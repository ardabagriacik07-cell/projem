using System.ComponentModel.DataAnnotations;

public class UyeSifreKodGonderViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
