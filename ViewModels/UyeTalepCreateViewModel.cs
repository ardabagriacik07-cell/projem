using System.ComponentModel.DataAnnotations;

public class UyeTalepCreateViewModel
{
    [Required]
    [StringLength(70)]
    public string Marka { get; set; } = string.Empty;

    [Required]
    [StringLength(90)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string ArizaAciklama { get; set; } = string.Empty;
}
