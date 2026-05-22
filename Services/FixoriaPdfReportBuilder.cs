using System.Globalization;
using System.Text;

public static class FixoriaPdfReportBuilder
{
    private const double PageWidth = 595;
    private const double PageHeight = 842;
    private const double Margin = 70;
    private static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

    public static byte[] BuildServiceReceiptReport(ServisKaydi servis)
    {
        var page = new StringBuilder();
        DrawReceiptBackground(page);
        DrawReceiptHeader(page, servis);
        DrawReceiptBody(page, servis);
        DrawReceiptFooter(page);
        return BuildPdf(new List<StringBuilder> { page });
    }

    public static byte[] BuildServiceHistoryReport(IEnumerable<ServisKaydi> servisler, AdminGecmisViewModel filtre)
    {
        var pages = new List<StringBuilder>();
        var page = NewHistoryPage(pages.Count + 1, filtre);
        pages.Add(page);
        var y = 660d;

        var items = servisler.ToList();
        if (items.Count == 0)
        {
            DrawText(page, "Kayıt bulunamadı.", Margin, y, 12, "0.20 0.27 0.38");
        }
        else
        {
            foreach (var servis in items)
            {
                if (y < 120)
                {
                    page = NewHistoryPage(pages.Count + 1, filtre);
                    pages.Add(page);
                    y = 660d;
                }

                DrawHistoryBlock(page, servis, y);
                y -= 116;
            }
        }

        return BuildPdf(pages);
    }

    private static void DrawReceiptBackground(StringBuilder page)
    {
        FillRect(page, 0, 0, PageWidth, PageHeight, "0.96 0.97 0.99");
        FillRect(page, 70, 702, 455, 96, "0.01 0.05 0.13");
        FillRect(page, 70, 699, 455, 4, "0.00 0.84 0.96");
        StrokeRect(page, 70, 80, 455, 718, "0.82 0.88 0.95", .8);

        page.AppendLine("0.02 0.14 0.36 RG");
        page.AppendLine("0.6 w");
        page.AppendLine("300 740 m 440 815 l S");
        page.AppendLine("0.00 0.65 0.95 RG");
        page.AppendLine("0.4 w");
        page.AppendLine("360 710 m 520 780 l S");
    }

    private static void DrawReceiptHeader(StringBuilder page, ServisKaydi servis)
    {
        DrawLogo(page, 96, 748);
        DrawText(page, "Fixoria", 130, 765, 20, "1 1 1", "F2");
        DrawText(page, "SERVIS TAKIP MERKEZI", 130, 750, 7, "0.16 0.86 0.95", "F2");

        DrawText(page, servis.Tarih.ToLocalTime().ToString("dd.MM.yyyy", Tr), 415, 765, 10, "1 1 1", "F2");
        DrawText(page, "İşlem Tarihi", 415, 752, 7, "0.66 0.78 0.92");
        DrawText(page, servis.Cihaz?.Musteri?.AdSoyad ?? "-", 415, 731, 10, "1 1 1", "F2", 26);
        DrawText(page, "Müşteri", 415, 718, 7, "0.66 0.78 0.92");
    }

    private static void DrawReceiptBody(StringBuilder page, ServisKaydi servis)
    {
        DrawText(page, "Servis İşlem Raporu", 96, 658, 17, "0.05 0.10 0.20", "F2");
        DrawText(page, "Bu fiş, seçili servis kaydında yapılan işlemleri ve fiyat bilgisini gösterir.", 96, 640, 8.5, "0.40 0.47 0.58");

        FillRect(page, 395, 630, 88, 48, "0.94 0.98 1");
        StrokeRect(page, 395, 630, 88, 48, "0.85 0.91 0.97", .6);
        DrawText(page, "TOPLAM TAHSILAT", 410, 659, 6.5, "0.40 0.47 0.58", "F2");
        DrawText(page, Money(servis.ToplamFiyat), 410, 641, 14, "0.05 0.10 0.20", "F2");

        DrawSummaryCard(page, 96, 594, "TARIH", servis.Tarih.ToLocalTime().ToString("dd.MM.yyyy", Tr));
        DrawSummaryCard(page, 197, 594, "MÜŞTERİ", servis.Cihaz?.Musteri?.AdSoyad ?? "-");
        DrawSummaryCard(page, 298, 594, "TOPLAM İŞLEM", servis.ServisIslemler.Count.ToString(CultureInfo.InvariantCulture));
        DrawSummaryCard(page, 399, 594, "RAPOR SAATI", DateTime.Now.ToString("dd.MM.yyyy HH:mm", Tr));

        DrawText(page, "İşlem Detayları", 96, 548, 12, "0.05 0.10 0.20", "F2");

        FillRect(page, 96, 250, 387, 280, "1 1 1");
        StrokeRect(page, 96, 250, 387, 280, "0.84 0.89 0.95", .6);
        FillRect(page, 96, 497, 387, 33, "0.96 0.98 1");
        FillRect(page, 96, 250, 3, 280, "0.00 0.48 1");

        DrawText(page, $"#{servis.Id}", 112, 510, 10, "0.00 0.35 0.95", "F2");
        DrawText(page, servis.Tarih.ToLocalTime().ToString("dd.MM.yyyy", Tr), 153, 512, 8.5, "0.18 0.25 0.36");
        DrawText(page, servis.Cihaz?.Musteri?.AdSoyad ?? "-", 214, 512, 8.5, "0.05 0.10 0.20", "F2", 28);
        DrawText(page, Money(servis.ToplamFiyat), 432, 512, 9, "0.00 0.35 0.95", "F2");

        var rowY = 474d;
        DrawDetailRow(page, "Cihaz", $"{servis.Cihaz?.Marka} {servis.Cihaz?.Model}".Trim(), ref rowY);
        DrawDetailRow(page, "Durum", servis.Durum, ref rowY);
        DrawDetailRow(page, "Onay", servis.FiyatOnayDurumu, ref rowY);
        DrawDetailRow(page, "Fiyat", Money(servis.ToplamFiyat), ref rowY);

        var islemler = servis.ServisIslemler.Where(x => x.Islem != null).ToList();
        var islemMetni = islemler.Count == 0
            ? "İşlem eklenmedi"
            : string.Join(" | ", islemler.Select(x => $"{x.Islem!.Ad}: {Money(x.Islem.Fiyat)}"));
        DrawDetailRow(page, "İşlemler", islemMetni, ref rowY, 2);
        DrawDetailRow(page, "Arıza Açıklaması", servis.Cihaz?.ArizaAciklama ?? "-", ref rowY, 2);
    }

    private static void DrawReceiptFooter(StringBuilder page)
    {
        DrawLogo(page, 100, 155, 22);
        DrawText(page, "Bizi tercih ettiğiniz için teşekkür ederiz!", 132, 166, 9.5, "0.05 0.10 0.20", "F2");
        DrawText(page, "Her türlü teknik destek ve servis ihtiyacınız için", 132, 153, 7, "0.40 0.47 0.58");
        DrawText(page, "bizimle iletişime geçebilirsiniz.", 132, 143, 7, "0.40 0.47 0.58");

        DrawText(page, "0850 123 45 67", 385, 166, 8, "0.05 0.10 0.20");
        DrawText(page, "info@fixoria.com", 385, 152, 8, "0.05 0.10 0.20");
        DrawText(page, "www.fixoria.com", 385, 138, 8, "0.05 0.10 0.20");
        DrawText(page, "İstanbul, Türkiye", 385, 124, 8, "0.05 0.10 0.20");

        FillRect(page, 70, 80, 455, 24, "0.01 0.05 0.13");
        DrawText(page, "2026 Fixoria Servis Takip Merkezi. Tüm hakları saklıdır.", 195, 89, 6.5, "0.70 0.80 0.92");
    }

    private static StringBuilder NewHistoryPage(int pageNumber, AdminGecmisViewModel filtre)
    {
        var page = new StringBuilder();
        FillRect(page, 0, 0, PageWidth, PageHeight, "0.96 0.97 0.99");
        FillRect(page, 54, 724, 487, 72, "0.01 0.05 0.13");
        FillRect(page, 54, 720, 487, 4, "0.00 0.84 0.96");
        DrawLogo(page, 76, 755, 22);
        DrawText(page, "Fixoria", 106, 766, 18, "1 1 1", "F2");
        DrawText(page, "Müşteri Servis Geçmişi", 106, 750, 8, "0.16 0.86 0.95", "F2");
        DrawText(page, $"Sayfa {pageNumber}", 455, 766, 8, "0.80 0.90 1");

        var tarih = FormatDate(filtre.Tarih) ?? "Tüm tarihler";
        DrawText(page, $"Filtre: {tarih} | Durum: {Safe(filtre.Durum, "Tüm durumlar")} | Arama: {Safe(filtre.Q, "Yok")}", 76, 692, 9, "0.35 0.43 0.54");
        return page;
    }

    private static void DrawHistoryBlock(StringBuilder page, ServisKaydi servis, double y)
    {
        FillRect(page, 76, y - 86, 443, 94, "1 1 1");
        StrokeRect(page, 76, y - 86, 443, 94, "0.84 0.89 0.95", .6);
        FillRect(page, 76, y - 86, 4, 94, "0.00 0.48 1");

        DrawText(page, $"#{servis.Id}  {servis.Tarih.ToLocalTime():dd.MM.yyyy}  {servis.Cihaz?.Musteri?.AdSoyad ?? "-"}", 92, y - 12, 11, "0.05 0.10 0.20", "F2", 58);
        DrawText(page, $"{servis.Cihaz?.Marka} {servis.Cihaz?.Model} | {servis.Durum} | {servis.FiyatOnayDurumu}", 92, y - 31, 8, "0.18 0.25 0.36", "F1", 72);
        DrawText(page, $"İşlem: {BuildOperationSummary(servis)}", 92, y - 48, 8, "0.30 0.38 0.50", "F1", 86);
        DrawText(page, $"Arıza: {servis.Cihaz?.ArizaAciklama ?? "-"}", 92, y - 64, 8, "0.30 0.38 0.50", "F1", 86);
        DrawText(page, Money(servis.ToplamFiyat), 452, y - 12, 10, "0.00 0.35 0.95", "F2");
    }

    private static void DrawSummaryCard(StringBuilder page, double x, double y, string label, string value)
    {
        FillRect(page, x, y, 86, 32, "1 1 1");
        StrokeRect(page, x, y, 86, 32, "0.86 0.91 0.97", .5);
        DrawText(page, label, x + 12, y + 19, 5.8, "0.40 0.47 0.58", "F2");
        DrawText(page, value, x + 12, y + 8, 7.2, "0.05 0.10 0.20", "F2", 17);
    }

    private static void DrawDetailRow(StringBuilder page, string label, string value, ref double y, int maxLines = 1)
    {
        DrawText(page, label, 116, y, 7.5, "0.30 0.38 0.50", "F2");
        var lines = Wrap(value, maxLines == 1 ? 58 : 72, maxLines);
        foreach (var line in lines)
        {
            DrawText(page, line, 214, y, 7.6, "0.05 0.10 0.20");
            y -= 13;
        }

        page.AppendLine("0.90 0.93 0.97 RG");
        page.AppendLine("0.35 w");
        page.AppendLine($"116 {Invariant(y + 4)} m 465 {Invariant(y + 4)} l S");
        y -= 9;
    }

    private static string BuildOperationSummary(ServisKaydi servis)
    {
        var operations = servis.ServisIslemler
            .Where(x => x.Islem != null)
            .Select(x => $"{x.Islem!.Ad} {Money(x.Islem.Fiyat)}")
            .ToList();
        return operations.Count == 0 ? "İşlem eklenmedi" : string.Join(" | ", operations);
    }

    private static byte[] BuildPdf(List<StringBuilder> pageContents)
    {
        var objects = new List<string>();
        var regularFontObject = 3 + pageContents.Count * 2;
        var boldFontObject = regularFontObject + 1;

        objects.Add("<< /Type /Catalog /Pages 2 0 R >>");
        var pageObjectNumbers = Enumerable.Range(0, pageContents.Count).Select(i => 3 + i * 2).ToList();
        objects.Add($"<< /Type /Pages /Kids [{string.Join(" ", pageObjectNumbers.Select(x => $"{x} 0 R"))}] /Count {pageContents.Count} >>");

        for (var i = 0; i < pageContents.Count; i++)
        {
            var pageObjectNumber = 3 + i * 2;
            var contentObjectNumber = pageObjectNumber + 1;
            objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth} {PageHeight}] /Resources << /Font << /F1 {regularFontObject} 0 R /F2 {boldFontObject} 0 R >> >> /Contents {contentObjectNumber} 0 R >>");
            var stream = pageContents[i].ToString();
            objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}endstream");
        }

        objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
        objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");

        var output = new StringBuilder();
        output.AppendLine("%PDF-1.4");
        output.AppendLine("%Fixoria");
        var offsets = new List<int> { 0 };

        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(output.ToString()));
            output.AppendLine($"{i + 1} 0 obj");
            output.AppendLine(objects[i]);
            output.AppendLine("endobj");
        }

        var xrefOffset = Encoding.ASCII.GetByteCount(output.ToString());
        output.AppendLine("xref");
        output.AppendLine($"0 {objects.Count + 1}");
        output.AppendLine("0000000000 65535 f ");
        for (var i = 1; i < offsets.Count; i++)
        {
            output.AppendLine($"{offsets[i]:D10} 00000 n ");
        }

        output.AppendLine("trailer");
        output.AppendLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
        output.AppendLine("startxref");
        output.AppendLine(xrefOffset.ToString(CultureInfo.InvariantCulture));
        output.AppendLine("%%EOF");
        return Encoding.ASCII.GetBytes(output.ToString());
    }

    private static void DrawLogo(StringBuilder page, double x, double y, double size = 26)
    {
        FillRect(page, x, y, size, size, "0.00 0.44 0.78");
        StrokeRect(page, x, y, size, size, "0.10 0.85 1", 1.1);
        DrawText(page, "F", x + size / 2 - 4, y + size / 2 - 4, size * .58, "0.75 0.96 1", "F2");
    }

    private static void FillRect(StringBuilder page, double x, double y, double width, double height, string rgb)
    {
        page.AppendLine($"{rgb} rg");
        page.AppendLine($"{Invariant(x)} {Invariant(y)} {Invariant(width)} {Invariant(height)} re f");
    }

    private static void StrokeRect(StringBuilder page, double x, double y, double width, double height, string rgb, double lineWidth)
    {
        page.AppendLine($"{rgb} RG");
        page.AppendLine($"{Invariant(lineWidth)} w");
        page.AppendLine($"{Invariant(x)} {Invariant(y)} {Invariant(width)} {Invariant(height)} re S");
    }

    private static void DrawText(StringBuilder page, string text, double x, double y, double size, string rgb, string font = "F1", int maxChars = 96)
    {
        page.AppendLine($"{rgb} rg");
        page.AppendLine("BT");
        page.AppendLine($"/{font} {Invariant(size)} Tf");
        page.AppendLine($"{Invariant(x)} {Invariant(y)} Td");
        page.AppendLine($"({Escape(text, maxChars)}) Tj");
        page.AppendLine("ET");
    }

    private static List<string> Wrap(string? text, int lineLength, int maxLines)
    {
        var safe = ToPdfSafe(text ?? "-");
        var words = safe.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        var current = new StringBuilder();

        foreach (var word in words)
        {
            if (current.Length + word.Length + 1 > lineLength)
            {
                if (current.Length > 0)
                {
                    lines.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    lines.Add(word.Length > lineLength ? word[..lineLength] : word);
                    if (lines.Count == maxLines)
                    {
                        break;
                    }

                    continue;
                }

                if (lines.Count == maxLines)
                {
                    break;
                }
            }

            if (current.Length > 0)
            {
                current.Append(' ');
            }
            current.Append(word);
        }

        if (current.Length > 0 && lines.Count < maxLines)
        {
            lines.Add(current.ToString());
        }

        if (lines.Count == 0)
        {
            lines.Add("-");
        }

        if (lines.Count == maxLines && words.Length > 0 && safe.Length > string.Join(" ", lines).Length)
        {
            lines[^1] = lines[^1].Length > 3 ? lines[^1][..Math.Min(lines[^1].Length, lineLength - 3)] + "..." : "...";
        }

        return lines;
    }

    private static string Escape(string text, int maxChars)
    {
        var normalized = ToPdfSafe(text);
        if (normalized.Length > maxChars)
        {
            normalized = normalized[..Math.Max(0, maxChars - 3)] + "...";
        }

        return normalized.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }

    private static string ToPdfSafe(string text)
    {
        var replacements = new Dictionary<char, char>
        {
            ['ç'] = 'c', ['Ç'] = 'C', ['ğ'] = 'g', ['Ğ'] = 'G',
            ['ı'] = 'i', ['İ'] = 'I', ['ö'] = 'o', ['Ö'] = 'O',
            ['ş'] = 's', ['Ş'] = 'S', ['ü'] = 'u', ['Ü'] = 'U',
            ['₺'] = 'T'
        };

        var builder = new StringBuilder();
        foreach (var ch in text)
        {
            if (replacements.TryGetValue(ch, out var replacement))
            {
                builder.Append(replacement);
            }
            else if (ch >= 32 && ch <= 126)
            {
                builder.Append(ch);
            }
            else
            {
                builder.Append(' ');
            }
        }

        return builder.ToString();
    }

    private static string Invariant(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
    private static string Money(decimal value) => $"{value.ToString("N0", Tr)} TL";
    private static string? FormatDate(DateTime? value) => value?.ToString("dd.MM.yyyy", Tr);
    private static string Safe(string? value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
}
