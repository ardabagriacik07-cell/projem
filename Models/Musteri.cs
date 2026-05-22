using System.ComponentModel.DataAnnotations;

public class Musteri
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [Display(Name = "Ad Soyad")]
    [StringLength(120)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Display(Name = "Telefon")]
    [StringLength(30)]
    public string Telefon { get; set; } = string.Empty;

    [Display(Name = "E-posta")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Şifre")]
    [StringLength(100)]
    public string Sifre { get; set; } = string.Empty;

    [Display(Name = "Üye Hesabı Var")]
    public bool UyeHesabiVar { get; set; }
    [Display(Name = "Kayıt Tarihi")]
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
    [Display(Name = "Son Giriş Tarihi")]
    public DateTime? SonGirisTarihi { get; set; }
    [StringLength(6)]
    [Display(Name = "Şifre Sıfırlama Kodu")]
    public string? SifreSifirlamaKodu { get; set; }
    [Display(Name = "Şifre Sıfırlama Kod Son Tarihi")]
    public DateTime? SifreSifirlamaKodSonTarih { get; set; }

    public List<Cihaz> Cihazlar { get; set; } = new();
    public List<MusteriBildirim> Bildirimler { get; set; } = new();
    public List<UyeYorum> Yorumlar { get; set; } = new();
}
