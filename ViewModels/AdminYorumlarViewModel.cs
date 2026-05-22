public class AdminYorumlarViewModel
{
    public int ToplamYorum { get; set; }
    public int YeniYorum { get; set; }
    public decimal OrtalamaPuan { get; set; }
    public List<UyeYorum> Yorumlar { get; set; } = new();
}
