using System.ComponentModel.DataAnnotations;

public class Musteri
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(120)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [StringLength(30)]
    public string Telefon { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Gecerli bir email giriniz.")]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [StringLength(100)]
    public string Sifre { get; set; } = string.Empty;

    public bool UyeHesabiVar { get; set; }
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? SonGirisTarihi { get; set; }
    [StringLength(6)]
    public string? SifreSifirlamaKodu { get; set; }
    public DateTime? SifreSifirlamaKodSonTarih { get; set; }

    public List<Cihaz> Cihazlar { get; set; } = new();
}
