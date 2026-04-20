using System.ComponentModel.DataAnnotations;

public class Cihaz
{
    public int Id { get; set; }
    public int MusteriId { get; set; }

    [Required(ErrorMessage = "Marka zorunludur.")]
    [StringLength(70)]
    public string Marka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model zorunludur.")]
    [StringLength(90)]
    public string Model { get; set; } = string.Empty;

    [StringLength(500)]
    public string ArizaAciklama { get; set; } = string.Empty;

    public Musteri? Musteri { get; set; }
    public List<ServisKaydi> ServisKayitlari { get; set; } = new();
}
