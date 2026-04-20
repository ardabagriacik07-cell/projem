using System.ComponentModel.DataAnnotations;

public class Admin
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string KullaniciAdi { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Sifre { get; set; } = string.Empty;
}
