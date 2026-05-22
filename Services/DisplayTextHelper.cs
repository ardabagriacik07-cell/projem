public static class DisplayTextHelper
{
    public static string Status(string? value)
    {
        return value switch
        {
            "Islemde" => "İşlemde",
            "Fiyat Onayi Bekliyor" => "Fiyat Onayı Bekliyor",
            "Tamamlandi" => "Tamamlandı",
            "Onay Bekliyor" => "Onay Bekliyor",
            "Onay Gerekmez" => "Onay Gerekmez",
            "Fiyat Reddedildi" => "Fiyat Reddedildi",
            "Reddedildi" => "Reddedildi",
            "Kabul Edildi" => "Kabul Edildi",
            "Teslim Edildi" => "Teslim Edildi",
            "Bekliyor" => "Bekliyor",
            null or "" => "-",
            _ => value
        };
    }
}
