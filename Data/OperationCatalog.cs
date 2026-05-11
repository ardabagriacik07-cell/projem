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
        new("Ekran Degisimi", "Telefon", 1800m, 6500m, "On cam saglamsa komple panel degisimi gerekebilir; marka ve OLED/AMOLED yapisina gore fiyat degisir."),
        new("Batarya Degisimi", "Telefon", 900m, 3200m, "Pil sagligi dusen veya sisen telefonlarda parca kalitesine gore fiyat araligi degisir."),
        new("Soket Tamiri", "Telefon", 700m, 2200m, "Sarj almama ve temassizlik sorunlarinda soket temizlik veya degisim uygulanir."),
        new("Genel Bakim", "Telefon", 600m, 1500m, "Ic temizlik, baglanti kontrolleri ve genel performans taramasi iceren temel servis bakimi."),
        new("Windows Format", "Yazilim & Destek", 750m, 1500m, "Temiz kurulum, surucu kontrolu ve temel sistem optimizasyonu dahil format hizmeti."),
        new("Driver Kurulumu", "Yazilim & Destek", 250m, 500m, "Chipset, ekran karti, ag ve cevre birimi suruculerinin uyumlu kurulumu."),
        new("Office Kurulumu", "Yazilim & Destek", 350m, 900m, "Office veya muadil ofis paketlerinin kurulum ve ilk ayar hizmeti."),
        new("Program Kurulumu", "Yazilim & Destek", 250m, 700m, "Temel kullanici programlari, lisansli yazilimlar veya arac kurulumu."),
        new("Virus Temizleme", "Yazilim & Destek", 900m, 2200m, "Zararli yazilim temizligi, tarama ve gerekli guvenlik sertlestirmeleri."),
        new("Sistem Hizlandirma", "Yazilim & Destek", 700m, 1800m, "Baslangic optimizasyonu, disk sagligi kontrolu ve performans ayari."),
        new("BIOS Guncelleme", "Laptop", 650m, 1600m, "BIOS/UEFI guncelleme, reset ve uyumluluk kontrolleri ile yapilan yazilim mudahalesi."),
        new("SSD Takma + Windows Kurulum", "Laptop", 2500m, 7500m, "SSD montaji, temiz Windows kurulumu ve temel surucu paketleri ile hizlandirma paketi."),
        new("RAM Yukseltme Isciligi", "Laptop", 700m, 2200m, "Mevcut cihaza uyumlu bellek montaji ve kararlilik testleri iceren iscilik bedeli."),
        new("HDD SSD Donusumu", "Laptop", 2500m, 7500m, "Mekanik diskten SSD'ye gecis, istege bagli veri klonlama ve performans optimizasyonu."),
        new("Termal Macun Degisimi", "Laptop", 600m, 1400m, "CPU/GPU isi transferini iyilestiren termal yenileme ve sicaklik testi."),
        new("Laptop Fan Temizligi", "Laptop", 600m, 1500m, "Toz temizligi, sogutma kanali bakimi ve fan ses/isinma kontrolu."),
        new("Laptop Komple Bakim", "Laptop", 1200m, 2800m, "Fan temizligi, termal yenileme, genel ic temizlik ve temel testleri kapsar."),
        new("Ekran Karti Temizligi", "Masaustu", 700m, 1800m, "GPU sogutucu, fan ve termal pad kontrolu ile temel temizlik bakimi."),
        new("Anakart Ariza Tespiti", "Laptop", 900m, 2200m, "Acilmama, kisa devre veya enerji dagitimi sorunlari icin teshis bedeli."),
        new("Laptop Ekran Degisimi", "Laptop", 1800m, 6500m, "Panel tipi, cozunurluk ve gaming serisi ekranlara gore fiyat degisen degisim hizmeti."),
        new("Laptop Batarya Degisimi", "Laptop", 1300m, 4500m, "Laptop pil degisimi; hucre yapisi ve model bagimsiz genel fiyat bandi."),
        new("Sarj Soketi Tamiri", "Laptop", 900m, 2200m, "DC jack soketi, lehim ve guc hattina yonelik sarj problemi onarimi."),
        new("Mentese Tamiri", "Laptop", 1200m, 3500m, "Kapak acma kapama sorunu, kirik menteseler ve kasa baglantilarinin guclendirilmesi."),
        new("Veri Kurtarma", "Yazilim & Destek", 1500m, 5000m, "Silinen veya okunmayan veriler icin mantiksal kurtarma ve ilk teshis dahil aralik."),
        new("FPS Oyun Performans Optimizasyonu", "Yazilim & Destek", 900m, 2500m, "Surucu, guc, termal ve oyun ici ayarlarla performans odakli optimizasyon hizmeti."),
        new("Sivi Temasi Temizligi", "Laptop", 1350m, 4500m, "Sivi temasli kart ve baglantilarda ultrasonik/kimyasal temizlik ve ilk onarim islemleri."),
        new("Kasa Toplama", "Masaustu", 1500m, 4500m, "Parca montaji, kablolama, BIOS ayari ve stres testi dahil sistem toplama hizmeti."),
        new("PSU Degisimi", "Masaustu", 800m, 2500m, "Guc kaynagi degisimi ve voltaj kararlilik kontrolu; parca sinifina gore fiyatlanir."),
        new("Ekran Karti Montaji", "Masaustu", 600m, 1500m, "GPU montaji, surucu kurulumu ve guc baglantilarinin test edilmesi."),
        new("Monitor Ariza Tespiti", "Masaustu", 600m, 1500m, "Panel, backlight veya guc karti kaynakli sorunlarda ilk teshis hizmeti."),
        new("Telefon Ekran Degisimi", "Telefon", 1800m, 8000m, "OLED, AMOLED ve premium panel cihazlarda modele gore genisleyen fiyat araligi."),
        new("Telefon Batarya Degisimi", "Telefon", 900m, 3200m, "Pil sagligi dusuk veya sisme yapan telefonlarda degisim fiyat araligi."),
        new("Telefon Sarj Soketi Degisimi", "Telefon", 700m, 2200m, "Sarj almama, kabloyu tutmama ve veri hattı sorunlarinda soket degisimi."),
        new("Kamera Degisimi", "Telefon", 1200m, 4500m, "Arka kamera modulu veya odaklama arizalarinda uygulanan degisim hizmeti."),
        new("Telefon Arka Kapak Degisimi", "Telefon", 1500m, 3500m, "Arka cam veya kapak kiriklarinda model bazli kasa arkasi degisim hizmeti."),
        new("Hoparlor Degisimi", "Telefon", 700m, 1800m, "Alt hoparlor veya zil sesi bozulmalarinda yapilan parca degisimi."),
        new("Ahize Degisimi", "Telefon", 700m, 1800m, "Gorusme sirasinda ses gelmeme veya cizirtida ahize degisimi uygulanir."),
        new("Yazilim Guncelleme", "Telefon", 600m, 1500m, "Guncelleme hatalari, boot loop ve surum gecislerinde yazilim mudahalesi."),
        new("Telefon Formatlama", "Telefon", 500m, 1200m, "Sifirlama, kurulum ve temel hesap ayarlariyla yapilan temiz yazilim kurulumu."),
        new("iCloud FRP Sorun Tespiti", "Telefon", 500m, 1200m, "Hesap kilidi kaynakli yazilim sorunlari icin yasal sinirlar icinde ariza tespiti."),
        new("Telefon Veri Yedekleme", "Telefon", 500m, 1500m, "Rehber, medya ve uygulama verilerinin aktarimi veya guvenli yedeklenmesi."),
        new("Face ID Tamiri", "Telefon", 2500m, 7000m, "TrueDepth veya ilgili sensor hattinda mikro onarim ve kalibrasyon gerektirebilir."),
        new("Parmak Izi Tamiri", "Telefon", 1200m, 3500m, "Home tusu veya yan sensor temasi kaynakli biyometrik ariza onarimi."),
        new("Anakart Tamiri", "Telefon", 2500m, 9000m, "Mikro lehim, enerji entegresi ve devre yolu sorunlarini kapsayan ileri onarim."),
        new("Telefon Sivi Temasi Onarimi", "Telefon", 1200m, 4500m, "Sivi temasinda kart temizligi, oksit giderme ve temel komponent onarimlari."),
        new("Kirilmaz Cam Takma", "Telefon", 250m, 700m, "Koruyucu cam uygulamasi ve hizalama hizmeti."),
        new("SIM Kart Okumama Sorunu", "Telefon", 900m, 2500m, "SIM okuyucu, tepsi veya anakart hattina bagli okuma sorunlarinin onarimi."),
        new("Telefon Genel Bakim", "Telefon", 600m, 1500m, "Ic temizlik, baglanti kontrolu ve temel fonksiyon testlerini kapsayan bakim."),
        new("Apple Samsung Yazilim Islemleri", "Telefon", 700m, 2200m, "Surum geri donusu, sistem kurtarma ve yazilim kararsizliklarinda destek hizmeti."),
        new("Ariza Tespit", "Yazilim & Destek", 400m, 1000m, "Donanim veya yazilim sorunlarini netlestirmek icin uygulanan genel teshis bedeli."),
        new("Telefon On Cam Degisimi", "Telefon", 1900m, 4000m, "Ic panel saglam cihazlarda cam laminasyon veya ust cam degisim hizmeti."),
        new("Kamera Cami Degisimi", "Telefon", 600m, 1500m, "Kamera lens caminda kirik veya cizik oldugunda uygulanan degisim islemi."),
        new("On Kamera Degisimi", "Telefon", 900m, 2500m, "On kamera modulu arizasinda parca degisimi ve odak testi uygulanir."),
        new("Mikrofon Degisimi", "Telefon", 700m, 1800m, "Ses gitmeme veya boguk ses problemlerinde mikrofon degisimi uygulanir."),
        new("Titresim Motoru Degisimi", "Telefon", 900m, 2200m, "Titresim geri bildirimi kesilen cihazlarda motor degisimi ve test yapilir."),
        new("Tablet Ekran Degisimi", "Tablet", 1800m, 12500m, "Tablet cam ve panel yapisina gore degisen ekran degisim hizmeti."),
        new("Tablet Batarya Degisimi", "Tablet", 950m, 9900m, "iPad ve Android tabletlerde kapasite ve kasa zorluguna gore fiyat degisir."),
        new("Tablet Sarj Soketi Tamiri", "Tablet", 500m, 5200m, "Sarj almama ve temassizlik sorunlarinda soket onarimi veya degisimi uygulanir."),
        new("Tablet Yazilim Onarimi", "Tablet", 300m, 3200m, "Acilmama, donma veya guncelleme hatalarinda yazilim kurtarma hizmeti."),
        new("Tablet Arka Kasa Degisimi", "Tablet", 1200m, 7000m, "Darbe veya egilme kaynakli arka govde ve kasa onarimlari."),
        new("Tablet Kamera Degisimi", "Tablet", 1000m, 3500m, "Tablet on/arka kamera modulu arizalarinda degisim hizmeti."),
        new("Tablet Dokunmatik Onarimi", "Tablet", 1900m, 6500m, "Cam + sensor katmaninda dokunmatik tepki sorunlarinin onarimi."),
        new("Laptop Klavye Degisimi", "Laptop", 1200m, 3500m, "Tusa basmama, sivi temasi veya eksik tus durumlarinda klavye degisimi."),
        new("Laptop Kamera Degisimi", "Laptop", 1300m, 2500m, "Bulanik, siyah ekran veren veya hic goruntusu olmayan webcam degisimi."),
        new("Laptop WiFi Karti Degisimi", "Laptop", 900m, 1800m, "Kablosuz ag karti veya Bluetooth baglanti sorunlarinda parca degisimi."),
        new("Laptop USB HDMI Port Tamiri", "Laptop", 700m, 1450m, "Kirilan veya temassiz portlar icin soket onarimi veya degisimi."),
        new("Laptop GPU Onarimi", "Laptop", 2200m, 6000m, "Goruntu sorunu ve isitma kaynakli GPU/VRAM arizalarinda teknik mudahale."),
        new("Masaustu Format ve Surucu Kurulumu", "Masaustu", 750m, 1500m, "Windows kurulumu, temel surucu paketi ve ilk sistem ayarlari."),
        new("Masaustu SSD Takma", "Masaustu", 2000m, 4500m, "SATA SSD montaji, BIOS kontrolu ve temel sistem optimizasyonu."),
        new("Masaustu NVMe Montaji", "Masaustu", 2500m, 6500m, "NVMe SSD montaji, uyumluluk ayari ve hiz testi dahil hizmet."),
        new("Masaustu Anakart Onarimi", "Masaustu", 1200m, 3500m, "Guc, kondansator veya chipset kaynakli arizalarda onarim hizmeti."),
        new("Masaustu CPU Degisimi", "Masaustu", 1000m, 4500m, "Islemci degisimi, sogutucu yeniden montaji ve stres testi dahil islem."),
        new("Masaustu Ag Baglantisi Onarimi", "Masaustu", 500m, 1200m, "Ethernet veya Wi-Fi baglanti sorunlarinin ayar ve parca bazli giderilmesi."),
        new("MacBook Ekran Degisimi", "MacBook", 6500m, 18000m, "Panel tipi ve model nesline gore genisleyen premium ekran degisim hizmeti."),
        new("MacBook Batarya Degisimi", "MacBook", 3200m, 7500m, "Pil modulu, yapiskan isciligi ve kalibrasyon gerektiren degisim hizmeti."),
        new("MacBook SSD Yukseltme", "MacBook", 3800m, 9000m, "Kapasite ve model uyumluluguna gore SSD degisim veya yukseltme hizmeti."),
        new("MacBook Fan Bakimi", "MacBook", 1800m, 3200m, "Fan temizligi ve termal yenileme ile sessiz ve stabil calisma bakimi."),
        new("MacBook Sarj Soketi Tamiri", "MacBook", 2800m, 5500m, "Type-C veya MagSafe guc girisi kaynakli sarj sorunlarinin onarimi."),
        new("MacBook Klavye Degisimi", "MacBook", 3500m, 9000m, "Tusa basmama veya sivi temasi kaynakli ust kasa/klavye degisim hizmeti."),
        new("MacBook Sivi Temasi Onarimi", "MacBook", 3500m, 9000m, "Anakart ve ust modullere etki eden sivi temaslarinda detayli onarim."),
        new("PS5 HDMI Soketi Tamiri", "Oyun Konsolu", 2000m, 3500m, "Goruntu vermeme ve soket kirigi sorunlarinda mikro lehimle HDMI onarimi."),
        new("PS5 Fan Bakimi", "Oyun Konsolu", 900m, 1800m, "Toz temizligi, sogutma kanali bakimi ve termal kontrol iceren servis."),
        new("PS5 SSD Yukseltme", "Oyun Konsolu", 2500m, 7000m, "Uyumlu M.2 SSD montaji ve performans/format kontrolleri ile yapilir."),
        new("PS5 Kol Analog Tamiri", "Oyun Konsolu", 900m, 2200m, "DualSense drift, analog modulu ve tus hassasiyet sorunlarinin onarimi."),
        new("PS5 Kol Sarj Soketi Tamiri", "Oyun Konsolu", 700m, 1600m, "Kolun sarj almamasi veya Type-C soket gevsemesi sorunlarinda onarim."),
        new("PS5 Yazilim Sistem Onarimi", "Oyun Konsolu", 1000m, 2500m, "Safe mode, sistem yazilimi ve kurulum sorunlarina yonelik yazilim destegi.")
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
