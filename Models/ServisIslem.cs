public class ServisIslem
{
    public int Id { get; set; }

    public int ServisKaydiId { get; set; }
    public int IslemId { get; set; }

    public ServisKaydi? ServisKaydi { get; set; }
    public Islem? Islem { get; set; }
}
