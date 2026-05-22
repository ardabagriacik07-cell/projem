using System.ComponentModel.DataAnnotations;

public class UyeYorumCreateViewModel
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [Display(Name = "Başlık")]
    [StringLength(120)]
    public string Baslik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yorum metni zorunludur.")]
    [Display(Name = "Yorum")]
    [StringLength(800)]
    public string Mesaj { get; set; } = string.Empty;

    [Display(Name = "Puan")]
    [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalı.")]
    public int Puan { get; set; } = 5;
}
