using System.ComponentModel.DataAnnotations;

public class Cihaz
{
    public int Id { get; set; }
    [Display(Name = "Müşteri")]
    public int MusteriId { get; set; }

    [Required(ErrorMessage = "Marka zorunludur.")]
    [Display(Name = "Marka")]
    [StringLength(70)]
    public string Marka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model zorunludur.")]
    [Display(Name = "Model")]
    [StringLength(90)]
    public string Model { get; set; } = string.Empty;

    [Display(Name = "Arıza Açıklaması")]
    [StringLength(500)]
    public string ArizaAciklama { get; set; } = string.Empty;

    public Musteri? Musteri { get; set; }
    public List<ServisKaydi> ServisKayitlari { get; set; } = new();
}
