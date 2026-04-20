using System.ComponentModel.DataAnnotations;

public class Islem
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Ad { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal Fiyat { get; set; }

    public List<ServisIslem> ServisIslemler { get; set; } = new();
}
