using System.Globalization;
using System.Text;

public sealed record OperationCatalogItem(
    string Name,
    string Category,
    decimal MinPrice,
    decimal MaxPrice,
    string Description)
{
    public decimal AveragePrice => Math.Round((MinPrice + MaxPrice) / 2m, 0, MidpointRounding.AwayFromZero);
}

public static class OperationCatalog
{
    public static IReadOnlyList<OperationCatalogItem> Items { get; } = new List<OperationCatalogItem>
    {
        new("Ekran Değişimi", "Telefon", 1800m, 6500m, "Ön cam sağlamsa komple panel değişimi gerekebilir; marka ve OLED/AMOLED yapısına göre fiyat değişir."),
        new("Batarya Değişimi", "Telefon", 900m, 3200m, "Pil sağlığı düşen veya şişen telefonlarda parça kalitesine göre fiyat aralığı değişir."),
        new("Soket Tamiri", "Telefon", 700m, 2200m, "Şarj almama ve temassızlık sorunlarında soket temizliği veya değişimi uygulanır."),
        new("Genel Bakım", "Telefon", 600m, 1500m, "İç temizlik, bağlantı kontrolleri ve genel performans taraması içeren temel servis bakımı."),
        new("Windows Format", "Yazılım & Destek", 750m, 1500m, "Temiz kurulum, sürücü kontrolü ve temel sistem optimizasyonu dahil format hizmeti."),
        new("Driver Kurulumu", "Yazılım & Destek", 250m, 500m, "Chipset, ekran kartı, ağ ve çevre birimi sürücülerinin uyumlu kurulumu."),
        new("Office Kurulumu", "Yazılım & Destek", 350m, 900m, "Office veya muadil ofis paketlerinin kurulum ve ilk ayar hizmeti."),
        new("Program Kurulumu", "Yazılım & Destek", 250m, 700m, "Temel kullanıcı programları, lisanslı yazılımlar veya araç kurulumu."),
        new("Virüs Temizleme", "Yazılım & Destek", 900m, 2200m, "Zararlı yazılım temizliği, tarama ve gerekli güvenlik sertleştirmeleri."),
        new("Sistem Hızlandırma", "Yazılım & Destek", 700m, 1800m, "Başlangıç optimizasyonu, disk sağlığı kontrolü ve performans ayarı."),
        new("BIOS Güncelleme", "Laptop", 650m, 1600m, "BIOS/UEFI güncelleme, reset ve uyumluluk kontrolleri ile yapılan yazılım müdahalesi."),
        new("SSD Takma + Windows Kurulum", "Laptop", 2500m, 7500m, "SSD montajı, temiz Windows kurulumu ve temel sürücü paketleri ile hızlandırma paketi."),
        new("RAM Yükseltme İşçiliği", "Laptop", 700m, 2200m, "Mevcut cihaza uyumlu bellek montajı ve kararlılık testleri içeren işçilik bedeli."),
        new("HDD SSD Dönüşümü", "Laptop", 2500m, 7500m, "Mekanik diskten SSD'ye geçiş, isteğe bağlı veri klonlama ve performans optimizasyonu."),
        new("Termal Macun Değişimi", "Laptop", 600m, 1400m, "CPU/GPU ısı transferini iyileştiren termal yenileme ve sıcaklık testi."),
        new("Laptop Fan Temizliği", "Laptop", 600m, 1500m, "Toz temizliği, soğutma kanalı bakımı ve fan ses/ısınma kontrolü."),
        new("Laptop Komple Bakım", "Laptop", 1200m, 2800m, "Fan temizliği, termal yenileme, genel iç temizlik ve temel testleri kapsar."),
        new("Ekran Kartı Temizliği", "Masaüstü", 700m, 1800m, "GPU soğutucu, fan ve termal pad kontrolü ile temel temizlik bakımı."),
        new("Anakart Arıza Tespiti", "Laptop", 900m, 2200m, "Açılmama, kısa devre veya enerji dağıtımı sorunları için teşhis bedeli."),
        new("Laptop Ekran Değişimi", "Laptop", 1800m, 6500m, "Panel tipi, çözünürlük ve gaming serisi ekranlara göre fiyatı değişen değişim hizmeti."),
        new("Laptop Batarya Değişimi", "Laptop", 1300m, 4500m, "Laptop pil değişimi; hücre yapısı ve model bağımsız genel fiyat bandı."),
        new("Şarj Soketi Tamiri", "Laptop", 900m, 2200m, "DC jack soketi, lehim ve güç hattına yönelik şarj problemi onarımı."),
        new("Menteşe Tamiri", "Laptop", 1200m, 3500m, "Kapak açma kapama sorunu, kırık menteşeler ve kasa bağlantılarının güçlendirilmesi."),
        new("Veri Kurtarma", "Yazılım & Destek", 1500m, 5000m, "Silinen veya okunmayan veriler için mantıksal kurtarma ve ilk teşhis dahil aralık."),
        new("FPS Oyun Performans Optimizasyonu", "Yazılım & Destek", 900m, 2500m, "Sürücü, güç, termal ve oyun içi ayarlarla performans odaklı optimizasyon hizmeti."),
        new("Sıvı Teması Temizliği", "Laptop", 1350m, 4500m, "Sıvı temaslı kart ve bağlantılarda ultrasonik/kimyasal temizlik ve ilk onarım işlemleri."),
        new("Kasa Toplama", "Masaüstü", 1500m, 4500m, "Parça montajı, kablolama, BIOS ayarı ve stres testi dahil sistem toplama hizmeti."),
        new("PSU Değişimi", "Masaüstü", 800m, 2500m, "Güç kaynağı değişimi ve voltaj kararlılık kontrolü; parça sınıfına göre fiyatlanır."),
        new("Ekran Kartı Montajı", "Masaüstü", 600m, 1500m, "GPU montajı, sürücü kurulumu ve güç bağlantılarının test edilmesi."),
        new("Monitör Arıza Tespiti", "Masaüstü", 600m, 1500m, "Panel, backlight veya güç kartı kaynaklı sorunlarda ilk teşhis hizmeti."),
        new("Telefon Ekran Değişimi", "Telefon", 1800m, 8000m, "OLED, AMOLED ve premium panel cihazlarda modele göre genişleyen fiyat aralığı."),
        new("Telefon Batarya Değişimi", "Telefon", 900m, 3200m, "Pil sağlığı düşük veya şişme yapan telefonlarda değişim fiyat aralığı."),
        new("Telefon Şarj Soketi Değişimi", "Telefon", 700m, 2200m, "Şarj almama, kabloyu tutmama ve veri hattı sorunlarında soket değişimi."),
        new("Kamera Değişimi", "Telefon", 1200m, 4500m, "Arka kamera modülü veya odaklama arızalarında uygulanan değişim hizmeti."),
        new("Telefon Arka Kapak Değişimi", "Telefon", 1500m, 3500m, "Arka cam veya kapak kırıklarında model bazlı kasa arkası değişim hizmeti."),
        new("Hoparlör Değişimi", "Telefon", 700m, 1800m, "Alt hoparlör veya zil sesi bozulmalarında yapılan parça değişimi."),
        new("Ahize Değişimi", "Telefon", 700m, 1800m, "Görüşme sırasında ses gelmeme veya cızırtıda ahize değişimi uygulanır."),
        new("Yazılım Güncelleme", "Telefon", 600m, 1500m, "Güncelleme hataları, boot loop ve sürüm geçişlerinde yazılım müdahalesi."),
        new("Telefon Formatlama", "Telefon", 500m, 1200m, "Sıfırlama, kurulum ve temel hesap ayarlarıyla yapılan temiz yazılım kurulumu."),
        new("iCloud FRP Sorun Tespiti", "Telefon", 500m, 1200m, "Hesap kilidi kaynaklı yazılım sorunları için yasal sınırlar içinde arıza tespiti."),
        new("Telefon Veri Yedekleme", "Telefon", 500m, 1500m, "Rehber, medya ve uygulama verilerinin aktarımı veya güvenli yedeklenmesi."),
        new("Face ID Tamiri", "Telefon", 2500m, 7000m, "TrueDepth veya ilgili sensör hattında mikro onarım ve kalibrasyon gerektirebilir."),
        new("Parmak İzi Tamiri", "Telefon", 1200m, 3500m, "Home tuşu veya yan sensör teması kaynaklı biyometrik arıza onarımı."),
        new("Anakart Tamiri", "Telefon", 2500m, 9000m, "Mikro lehim, enerji entegresi ve devre yolu sorunlarını kapsayan ileri onarım."),
        new("Telefon Sıvı Teması Onarımı", "Telefon", 1200m, 4500m, "Sıvı temasında kart temizliği, oksit giderme ve temel komponent onarımları."),
        new("Kırılmaz Cam Takma", "Telefon", 250m, 700m, "Koruyucu cam uygulaması ve hizalama hizmeti."),
        new("SIM Kart Okumama Sorunu", "Telefon", 900m, 2500m, "SIM okuyucu, tepsi veya anakart hattına bağlı okuma sorunlarının onarımı."),
        new("Telefon Genel Bakım", "Telefon", 600m, 1500m, "İç temizlik, bağlantı kontrolü ve temel fonksiyon testlerini kapsayan bakım."),
        new("Apple Samsung Yazılım İşlemleri", "Telefon", 700m, 2200m, "Sürüm geri dönüşü, sistem kurtarma ve yazılım kararsızlıklarında destek hizmeti."),
        new("Arıza Tespit", "Yazılım & Destek", 400m, 1000m, "Donanım veya yazılım sorunlarını netleştirmek için uygulanan genel teşhis bedeli."),
        new("Telefon Ön Cam Değişimi", "Telefon", 1900m, 4000m, "İç panel sağlam cihazlarda cam laminasyon veya üst cam değişim hizmeti."),
        new("Kamera Camı Değişimi", "Telefon", 600m, 1500m, "Kamera lens camında kırık veya çizik olduğunda uygulanan değişim işlemi."),
        new("Ön Kamera Değişimi", "Telefon", 900m, 2500m, "Ön kamera modülü arızasında parça değişimi ve odak testi uygulanır."),
        new("Mikrofon Değişimi", "Telefon", 700m, 1800m, "Ses gitmeme veya boğuk ses problemlerinde mikrofon değişimi uygulanır."),
        new("Titreşim Motoru Değişimi", "Telefon", 900m, 2200m, "Titreşim geri bildirimi kesilen cihazlarda motor değişimi ve test yapılır."),
        new("Tablet Ekran Değişimi", "Tablet", 1800m, 12500m, "Tablet cam ve panel yapısına göre değişen ekran değişim hizmeti."),
        new("Tablet Batarya Değişimi", "Tablet", 950m, 9900m, "iPad ve Android tabletlerde kapasite ve kasa zorluğuna göre fiyat değişir."),
        new("Tablet Şarj Soketi Tamiri", "Tablet", 500m, 5200m, "Şarj almama ve temassızlık sorunlarında soket onarımı veya değişimi uygulanır."),
        new("Tablet Yazılım Onarımı", "Tablet", 300m, 3200m, "Açılmama, donma veya güncelleme hatalarında yazılım kurtarma hizmeti."),
        new("Tablet Arka Kasa Değişimi", "Tablet", 1200m, 7000m, "Darbe veya eğilme kaynaklı arka gövde ve kasa onarımları."),
        new("Tablet Kamera Değişimi", "Tablet", 1000m, 3500m, "Tablet ön/arka kamera modülü arızalarında değişim hizmeti."),
        new("Tablet Dokunmatik Onarımı", "Tablet", 1900m, 6500m, "Cam + sensör katmanında dokunmatik tepki sorunlarının onarımı."),
        new("Laptop Klavye Değişimi", "Laptop", 1200m, 3500m, "Tuşa basmama, sıvı teması veya eksik tuş durumlarında klavye değişimi."),
        new("Laptop Kamera Değişimi", "Laptop", 1300m, 2500m, "Bulanık, siyah ekran veren veya hiç görüntüsü olmayan webcam değişimi."),
        new("Laptop WiFi Kartı Değişimi", "Laptop", 900m, 1800m, "Kablosuz ağ kartı veya Bluetooth bağlantı sorunlarında parça değişimi."),
        new("Laptop USB HDMI Port Tamiri", "Laptop", 700m, 1450m, "Kırılan veya temassız portlar için soket onarımı veya değişimi."),
        new("Laptop GPU Onarımı", "Laptop", 2200m, 6000m, "Görüntü sorunu ve ısınma kaynaklı GPU/VRAM arızalarında teknik müdahale."),
        new("Masaüstü Format ve Sürücü Kurulumu", "Masaüstü", 750m, 1500m, "Windows kurulumu, temel sürücü paketi ve ilk sistem ayarları."),
        new("Masaüstü SSD Takma", "Masaüstü", 2000m, 4500m, "SATA SSD montajı, BIOS kontrolü ve temel sistem optimizasyonu."),
        new("Masaüstü NVMe Montajı", "Masaüstü", 2500m, 6500m, "NVMe SSD montajı, uyumluluk ayarı ve hız testi dahil hizmet."),
        new("Masaüstü Anakart Onarımı", "Masaüstü", 1200m, 3500m, "Güç, kondansatör veya chipset kaynaklı arızalarda onarım hizmeti."),
        new("Masaüstü CPU Değişimi", "Masaüstü", 1000m, 4500m, "İşlemci değişimi, soğutucu yeniden montajı ve stres testi dahil işlem."),
        new("Masaüstü Ağ Bağlantısı Onarımı", "Masaüstü", 500m, 1200m, "Ethernet veya Wi-Fi bağlantı sorunlarının ayar ve parça bazlı giderilmesi."),
        new("MacBook Ekran Değişimi", "MacBook", 6500m, 18000m, "Panel tipi ve model nesline göre genişleyen premium ekran değişim hizmeti."),
        new("MacBook Batarya Değişimi", "MacBook", 3200m, 7500m, "Pil modülü, yapışkan işçiliği ve kalibrasyon gerektiren değişim hizmeti."),
        new("MacBook SSD Yükseltme", "MacBook", 3800m, 9000m, "Kapasite ve model uyumluluğuna göre SSD değişim veya yükseltme hizmeti."),
        new("MacBook Fan Bakımı", "MacBook", 1800m, 3200m, "Fan temizliği ve termal yenileme ile sessiz ve stabil çalışma bakımı."),
        new("MacBook Şarj Soketi Tamiri", "MacBook", 2800m, 5500m, "Type-C veya MagSafe güç girişi kaynaklı şarj sorunlarının onarımı."),
        new("MacBook Klavye Değişimi", "MacBook", 3500m, 9000m, "Tuşa basmama veya sıvı teması kaynaklı üst kasa/klavye değişim hizmeti."),
        new("MacBook Sıvı Teması Onarımı", "MacBook", 3500m, 9000m, "Anakart ve üst modüllere etki eden sıvı temaslarında detaylı onarım."),
        new("PS5 HDMI Soketi Tamiri", "Oyun Konsolu", 2000m, 3500m, "Görüntü vermeme ve soket kırığı sorunlarında mikro lehimle HDMI onarımı."),
        new("PS5 Fan Bakımı", "Oyun Konsolu", 900m, 1800m, "Toz temizliği, soğutma kanalı bakımı ve termal kontrol içeren servis."),
        new("PS5 SSD Yükseltme", "Oyun Konsolu", 2500m, 7000m, "Uyumlu M.2 SSD montajı ve performans/format kontrolleri ile yapılır."),
        new("PS5 Kol Analog Tamiri", "Oyun Konsolu", 900m, 2200m, "DualSense drift, analog modülü ve tuş hassasiyet sorunlarının onarımı."),
        new("PS5 Kol Şarj Soketi Tamiri", "Oyun Konsolu", 700m, 1600m, "Kolun şarj almaması veya Type-C soket gevşemesi sorunlarında onarım."),
        new("PS5 Yazılım Sistem Onarımı", "Oyun Konsolu", 1000m, 2500m, "Safe mode, sistem yazılımı ve kurulum sorunlarına yönelik yazılım desteği.")
    };

    public static IReadOnlyList<string> Categories => Items
        .Select(x => x.Category)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(x => x)
        .ToList();

    public static string NormalizeKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var turkishNormalized = value
            .Replace('I', 'i')
            .Replace('İ', 'i')
            .Replace('ı', 'i')
            .Replace('Ş', 's')
            .Replace('ş', 's')
            .Replace('Ğ', 'g')
            .Replace('ğ', 'g')
            .Replace('Ü', 'u')
            .Replace('ü', 'u')
            .Replace('Ö', 'o')
            .Replace('ö', 'o')
            .Replace('Ç', 'c')
            .Replace('ç', 'c');

        var normalized = turkishNormalized.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(char.ToLowerInvariant(ch));
            }
        }

        return builder.ToString();
    }
}
