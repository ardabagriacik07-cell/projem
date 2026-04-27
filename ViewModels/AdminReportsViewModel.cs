public class AdminReportsViewModel
{
    public decimal BuAyCiro { get; set; }
    public decimal GecenAyCiro { get; set; }
    public int BuAyServisSayisi { get; set; }
    public int TamamlananServisSayisi { get; set; }
    public List<AdminMonthlyRevenuePointViewModel> Son6AyCiro { get; set; } = new();
    public List<AdminBrandInsightViewModel> MarkaPerformansi { get; set; } = new();
    public List<AdminCustomerValueViewModel> EnDegerliMusteriler { get; set; } = new();
    public List<AdminStatusBreakdownItem> DurumDagilimi { get; set; } = new();
}
