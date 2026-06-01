using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/mobile")]
public class MobileApiController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IEmailSender _emailSender;

    public MobileApiController(AppDbContext db, IEmailSender emailSender)
    {
        _db = db;
        _emailSender = emailSender;
    }

    [HttpPost("member/login")]
    public async Task<ActionResult<MemberSyncResponse>> MemberLogin(MemberLoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var member = await _db.Musteriler
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.UyeHesabiVar && x.Sifre == request.Password);

        if (member == null)
        {
            return BadRequest(new ApiMessageResponse("E-posta veya şifre hatalı."));
        }

        member.SonGirisTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await BuildMemberSyncAsync(member.Id, string.Empty);
    }

    [HttpPost("member/register")]
    public async Task<ActionResult<MemberSyncResponse>> MemberRegister(MemberRegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await _db.Musteriler.AnyAsync(x => x.Email.ToLower() == email);
        if (exists)
        {
            return BadRequest(new ApiMessageResponse("Bu e-posta zaten kullanılıyor."));
        }

        var member = new Musteri
        {
            AdSoyad = request.FullName.Trim(),
            Telefon = request.Phone.Trim(),
            Email = email,
            Sifre = request.Password,
            UyeHesabiVar = true,
            KayitTarihi = DateTime.UtcNow,
            SonGirisTarihi = DateTime.UtcNow
        };

        _db.Musteriler.Add(member);
        await _db.SaveChangesAsync();

        return await BuildMemberSyncAsync(member.Id, string.Empty);
    }

    [HttpPost("member/password-reset-code")]
    public async Task<ActionResult<ApiMessageResponse>> SendMemberPasswordResetCode(PasswordResetCodeRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var member = await _db.Musteriler.FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.UyeHesabiVar);
        if (member == null)
        {
            return BadRequest(new ApiMessageResponse("Bu e-posta ile kayıtlı aktif bir üye bulunamadı."));
        }

        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        member.SifreSifirlamaKodu = code;
        member.SifreSifirlamaKodSonTarih = DateTime.UtcNow.AddMinutes(10);
        await _db.SaveChangesAsync();

        var body = $"""
            <div style="font-family:Arial,sans-serif;font-size:16px;color:#111827">
                <p>Merhaba {member.AdSoyad},</p>
                <p>Şifre sıfırlama kodun aşağıdadır:</p>
                <p style="font-size:32px;font-weight:700;letter-spacing:6px;margin:16px 0;">{code}</p>
                <p>Bu kod 10 dakika boyunca geçerlidir.</p>
            </div>
            """;

        try
        {
            await _emailSender.SendAsync(member.Email, "Fixoria Şifre Sıfırlama Kodu", body);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiMessageResponse($"Kod oluşturuldu ancak e-posta gönderilemedi: {ex.Message}"));
        }

        return Ok(new ApiMessageResponse("Kod e-posta adresine gönderildi."));
    }

    [HttpPost("member/password-reset")]
    public async Task<ActionResult<ApiMessageResponse>> ResetMemberPassword(PasswordResetRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var code = request.Code.Trim();
        var member = await _db.Musteriler.FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.UyeHesabiVar);
        if (member == null)
        {
            return BadRequest(new ApiMessageResponse("Üye hesabı bulunamadı."));
        }

        if (member.SifreSifirlamaKodSonTarih.HasValue == false ||
            member.SifreSifirlamaKodSonTarih.Value < DateTime.UtcNow ||
            string.Equals(member.SifreSifirlamaKodu, code, StringComparison.Ordinal) == false)
        {
            return BadRequest(new ApiMessageResponse("Kod hatalı veya süresi dolmuş."));
        }

        member.Sifre = request.NewPassword;
        member.SifreSifirlamaKodu = null;
        member.SifreSifirlamaKodSonTarih = null;
        await _db.SaveChangesAsync();

        return Ok(new ApiMessageResponse("Şifren başarıyla güncellendi."));
    }

    [HttpGet("member/{memberId:int}/snapshot")]
    public async Task<ActionResult<MemberSyncResponse>> GetMemberSnapshot(int memberId)
    {
        return await BuildMemberSyncAsync(memberId, string.Empty);
    }

    [HttpPut("member/{memberId:int}/profile")]
    public async Task<ActionResult<MemberSyncResponse>> UpdateMemberProfile(int memberId, UpdateMemberProfileRequest request)
    {
        var member = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == memberId && x.UyeHesabiVar);
        if (member == null)
        {
            return NotFound(new ApiMessageResponse("Üye hesabı bulunamadı."));
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var emailExists = await _db.Musteriler.AnyAsync(x => x.Id != memberId && x.Email.ToLower() == email);
        if (emailExists)
        {
            return BadRequest(new ApiMessageResponse("Bu e-posta başka bir hesapta kullanılıyor."));
        }

        member.AdSoyad = request.FullName.Trim();
        member.Telefon = request.Phone.Trim();
        member.Email = email;
        if (string.IsNullOrWhiteSpace(request.NewPassword) == false)
        {
            member.Sifre = request.NewPassword;
        }

        await _db.SaveChangesAsync();
        return await BuildMemberSyncAsync(memberId, "Profilin güncellendi.");
    }

    [HttpPost("member/{memberId:int}/service-requests")]
    public async Task<ActionResult<MemberSyncResponse>> CreateMemberServiceRequest(int memberId, CreateMemberServiceRequestRequest request)
    {
        var member = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == memberId && x.UyeHesabiVar);
        if (member == null)
        {
            return NotFound(new ApiMessageResponse("Aktif üye bulunamadı."));
        }

        var device = new Cihaz
        {
            MusteriId = memberId,
            Marka = request.Brand.Trim(),
            Model = request.Model.Trim(),
            ArizaAciklama = request.IssueDescription.Trim()
        };

        _db.Cihazlar.Add(device);
        await _db.SaveChangesAsync();

        var service = new ServisKaydi
        {
            CihazId = device.Id,
            Tarih = DateTime.UtcNow,
            Durum = "Bekliyor",
            ToplamFiyat = 0
        };

        _db.ServisKayitlari.Add(service);
        await _db.SaveChangesAsync();

        return await BuildMemberSyncAsync(memberId, "Talebin alındı. En kısa sürede inceleyeceğiz.");
    }

    [HttpPost("member/{memberId:int}/services/{serviceId:int}/price-offer/accept")]
    public async Task<ActionResult<MemberSyncResponse>> AcceptMemberServicePriceOffer(int memberId, int serviceId)
    {
        var service = await GetMemberServiceAsync(memberId, serviceId);
        if (service == null)
        {
            return NotFound(new ApiMessageResponse("Servis kaydı bulunamadı."));
        }

        if (service.FiyatOnayDurumu != "Onay Bekliyor")
        {
            return BadRequest(new ApiMessageResponse("Bu servis için bekleyen fiyat onayı yok."));
        }

        service.FiyatOnayDurumu = "Kabul Edildi";
        service.FiyatCevapTarihi = DateTime.UtcNow;
        service.Durum = "Islemde";
        await _db.SaveChangesAsync();

        return await BuildMemberSyncAsync(memberId, "Fiyat teklifini kabul ettin. Servis işleme alındı.");
    }

    [HttpPost("member/{memberId:int}/services/{serviceId:int}/price-offer/reject")]
    public async Task<ActionResult<MemberSyncResponse>> RejectMemberServicePriceOffer(int memberId, int serviceId)
    {
        var service = await GetMemberServiceAsync(memberId, serviceId);
        if (service == null)
        {
            return NotFound(new ApiMessageResponse("Servis kaydı bulunamadı."));
        }

        if (service.FiyatOnayDurumu != "Onay Bekliyor")
        {
            return BadRequest(new ApiMessageResponse("Bu servis için bekleyen fiyat onayı yok."));
        }

        service.FiyatOnayDurumu = "Reddedildi";
        service.FiyatCevapTarihi = DateTime.UtcNow;
        service.Durum = "Fiyat Reddedildi";
        await _db.SaveChangesAsync();

        return await BuildMemberSyncAsync(memberId, "Fiyat teklifini reddettin.");
    }

    [HttpPost("member/{memberId:int}/reviews")]
    public async Task<ActionResult<MemberSyncResponse>> CreateMemberReview(int memberId, CreateMemberReviewRequest request)
    {
        var member = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == memberId && x.UyeHesabiVar);
        if (member == null)
        {
            return NotFound(new ApiMessageResponse("Aktif üye bulunamadı."));
        }

        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.Message) ||
            request.Rating < 1 ||
            request.Rating > 5)
        {
            return BadRequest(new ApiMessageResponse("Başlık, yorum ve 1-5 arası puan gerekli."));
        }

        _db.UyeYorumlari.Add(new UyeYorum
        {
            MusteriId = memberId,
            Baslik = request.Title.Trim(),
            Mesaj = request.Message.Trim(),
            Puan = request.Rating,
            OlusturmaTarihi = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return await BuildMemberSyncAsync(memberId, "Yorumun alındı. Teşekkür ederiz.");
    }

    [HttpPost("member/{memberId:int}/notifications/{notificationId:int}/read")]
    public async Task<ActionResult<MemberSyncResponse>> MarkMemberNotificationRead(int memberId, int notificationId)
    {
        var notification = await _db.MusteriBildirimleri
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.MusteriId == memberId);

        if (notification == null)
        {
            return NotFound(new ApiMessageResponse("Bildirim bulunamadı."));
        }

        notification.Okundu = true;
        notification.OkunmaTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await BuildMemberSyncAsync(memberId, "Bildirim okundu olarak işaretlendi.");
    }

    [HttpPost("member/{memberId:int}/notifications/read-all")]
    public async Task<ActionResult<MemberSyncResponse>> MarkAllMemberNotificationsRead(int memberId)
    {
        var notifications = await _db.MusteriBildirimleri
            .Where(x => x.MusteriId == memberId && x.Okundu == false)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.Okundu = true;
            notification.OkunmaTarihi = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return await BuildMemberSyncAsync(memberId, "Bildirimler okundu olarak işaretlendi.");
    }

    [HttpPost("admin/login")]
    public async Task<ActionResult<AdminSyncResponse>> AdminLogin(AdminLoginRequest request)
    {
        var username = request.Username.Trim();
        var admin = await _db.Adminler.FirstOrDefaultAsync(x => x.KullaniciAdi == username && x.Sifre == request.Password);
        if (admin == null)
        {
            return BadRequest(new ApiMessageResponse("Kullanıcı adı veya şifre hatalı."));
        }

        return await BuildAdminSyncAsync(admin.KullaniciAdi, string.Empty);
    }

    [HttpGet("admin/snapshot")]
    public async Task<ActionResult<AdminSyncResponse>> GetAdminSnapshot([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest(new ApiMessageResponse("Yönetici kullanıcı adı gerekli."));
        }

        var adminExists = await _db.Adminler.AnyAsync(x => x.KullaniciAdi == username);
        if (adminExists == false)
        {
            return NotFound(new ApiMessageResponse("Yönetici bulunamadı."));
        }

        return await BuildAdminSyncAsync(username, string.Empty);
    }

    [HttpPost("admin/password")]
    public async Task<ActionResult<AdminSyncResponse>> ChangeAdminPassword(ChangeAdminPasswordRequest request)
    {
        var username = request.AdminUsername.Trim();
        var admin = await _db.Adminler.FirstOrDefaultAsync(x => x.KullaniciAdi == username);
        if (admin == null)
        {
            return NotFound(new ApiMessageResponse("Yönetici bulunamadı."));
        }

        if (admin.Sifre != request.CurrentPassword)
        {
            return BadRequest(new ApiMessageResponse("Mevcut şifre doğru değil."));
        }

        var newUsername = string.IsNullOrWhiteSpace(request.NewUsername)
            ? admin.KullaniciAdi
            : request.NewUsername.Trim();

        if (string.IsNullOrWhiteSpace(newUsername))
        {
            return BadRequest(new ApiMessageResponse("Yeni kullanıcı adı boş olamaz."));
        }

        if (newUsername.Length < 3 || newUsername.Length > 50)
        {
            return BadRequest(new ApiMessageResponse("Yeni kullanıcı adı 3-50 karakter arasında olmalı."));
        }

        var usernameExists = await _db.Adminler.AnyAsync(x =>
            x.Id != admin.Id && x.KullaniciAdi.ToLower() == newUsername.ToLower());

        if (usernameExists)
        {
            return BadRequest(new ApiMessageResponse("Bu kullanıcı adı başka bir yönetici tarafından kullanılıyor."));
        }

        var passwordWillChange = string.IsNullOrWhiteSpace(request.NewPassword) == false;
        if (passwordWillChange && request.NewPassword!.Length < 5)
        {
            return BadRequest(new ApiMessageResponse("Yeni şifre en az 5 karakter olmalı."));
        }

        admin.KullaniciAdi = newUsername;
        if (passwordWillChange)
        {
            admin.Sifre = request.NewPassword!;
        }

        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(admin.KullaniciAdi, passwordWillChange
            ? "Yönetici hesabı güncellendi."
            : "Yönetici kullanıcı adı güncellendi.");
    }

    [HttpPost("admin/reviews/{reviewId:int}/read")]
    public async Task<ActionResult<AdminSyncResponse>> MarkAdminReviewRead(int reviewId, DeleteAdminEntityRequest request)
    {
        var review = await _db.UyeYorumlari.FindAsync(reviewId);
        if (review == null)
        {
            return NotFound(new ApiMessageResponse("Yorum bulunamadı."));
        }

        review.YoneticiGordu = true;
        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "Yorum okundu olarak işaretlendi.");
    }

    [HttpPost("admin/reviews/{reviewId:int}/delete")]
    public async Task<ActionResult<AdminSyncResponse>> DeleteAdminReview(int reviewId, DeleteAdminEntityRequest request)
    {
        var review = await _db.UyeYorumlari.FindAsync(reviewId);
        if (review == null)
        {
            return NotFound(new ApiMessageResponse("Yorum bulunamadı."));
        }

        _db.UyeYorumlari.Remove(review);
        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "Yorum silindi.");
    }

    [HttpPost("admin/members")]
    public async Task<ActionResult<AdminSyncResponse>> CreateAdminMember(CreateAdminMemberRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await _db.Musteriler.AnyAsync(x => x.Email.ToLower() == email);
        if (exists)
        {
            return BadRequest(new ApiMessageResponse("Bu e-posta zaten kullanılıyor."));
        }

        var member = new Musteri
        {
            AdSoyad = request.FullName.Trim(),
            Telefon = request.Phone.Trim(),
            Email = email,
            Sifre = request.CreateAccount ? "12345" : string.Empty,
            UyeHesabiVar = request.CreateAccount,
            KayitTarihi = DateTime.UtcNow
        };

        _db.Musteriler.Add(member);
        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "Müşteri oluşturuldu.");
    }

    [HttpPost("admin/devices")]
    public async Task<ActionResult<AdminSyncResponse>> CreateAdminDevice(CreateAdminDeviceRequest request)
    {
        var memberExists = await _db.Musteriler.AnyAsync(x => x.Id == request.MemberId);
        if (memberExists == false)
        {
            return NotFound(new ApiMessageResponse("Müşteri bulunamadı."));
        }

        var device = new Cihaz
        {
            MusteriId = request.MemberId,
            Marka = request.Brand.Trim(),
            Model = request.Model.Trim(),
            ArizaAciklama = request.IssueDescription.Trim()
        };

        _db.Cihazlar.Add(device);
        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "Cihaz oluşturuldu.");
    }

    [HttpPost("admin/services")]
    public async Task<ActionResult<AdminSyncResponse>> CreateAdminService(CreateAdminServiceRequest request)
    {
        var deviceExists = await _db.Cihazlar.AnyAsync(x => x.Id == request.DeviceId);
        if (deviceExists == false)
        {
            return NotFound(new ApiMessageResponse("Cihaz bulunamadı."));
        }

        var selectedActions = await _db.Islemler
            .Where(x => request.ActionIds.Contains(x.Id))
            .ToListAsync();

        var service = new ServisKaydi
        {
            CihazId = request.DeviceId,
            Tarih = DateTime.UtcNow,
            Durum = request.Status,
            ToplamFiyat = selectedActions.Sum(x => x.Fiyat),
            FiyatOnayDurumu = "Onay Gerekmez"
        };

        if (request.SendPriceApproval && service.ToplamFiyat > 0)
        {
            service.Durum = "Fiyat Onayi Bekliyor";
            service.FiyatOnayDurumu = "Onay Bekliyor";
            service.FiyatOnayTarihi = DateTime.UtcNow;
        }

        _db.ServisKayitlari.Add(service);
        await _db.SaveChangesAsync();

        foreach (var actionId in request.ActionIds.Distinct())
        {
            _db.ServisIslemler.Add(new ServisIslem
            {
                ServisKaydiId = service.Id,
                IslemId = actionId
            });
        }

        await _db.SaveChangesAsync();
        await ServisBildirimHelper.TamamlananServisBildiriminiOlusturAsync(_db, service.Id);
        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "Servis kaydı oluşturuldu.");
    }

    [HttpPut("admin/services/{serviceId:int}")]
    public async Task<ActionResult<AdminSyncResponse>> UpdateAdminService(int serviceId, UpdateAdminServiceRequest request)
    {
        var service = await _db.ServisKayitlari
            .Include(x => x.ServisIslemler)
            .FirstOrDefaultAsync(x => x.Id == serviceId);

        if (service == null)
        {
            return NotFound(new ApiMessageResponse("Servis kaydı bulunamadı."));
        }

        var deviceExists = await _db.Cihazlar.AnyAsync(x => x.Id == request.DeviceId);
        if (deviceExists == false)
        {
            return NotFound(new ApiMessageResponse("Cihaz bulunamadı."));
        }

        service.CihazId = request.DeviceId;
        service.Tarih = request.Date;
        service.Durum = request.Status.Trim();

        if (service.ServisIslemler.Count > 0)
        {
            _db.ServisIslemler.RemoveRange(service.ServisIslemler);
        }

        foreach (var actionId in request.ActionIds.Distinct())
        {
            _db.ServisIslemler.Add(new ServisIslem
            {
                ServisKaydiId = service.Id,
                IslemId = actionId
            });
        }

        var selectedActionTotal = await _db.Islemler
            .Where(x => request.ActionIds.Contains(x.Id))
            .SumAsync(x => (decimal?)x.Fiyat) ?? 0;

        service.ToplamFiyat = selectedActionTotal;

        if (request.SendPriceApproval && service.ToplamFiyat > 0)
        {
            service.Durum = "Fiyat Onayi Bekliyor";
            service.FiyatOnayDurumu = "Onay Bekliyor";
            service.FiyatOnayTarihi = DateTime.UtcNow;
            service.FiyatCevapTarihi = null;
        }
        else if (service.FiyatOnayDurumu == "Onay Bekliyor" && service.Durum != "Fiyat Onayi Bekliyor")
        {
            service.FiyatOnayDurumu = "Onay Gerekmez";
            service.FiyatOnayTarihi = null;
            service.FiyatCevapTarihi = null;
        }

        await _db.SaveChangesAsync();
        await ServisBildirimHelper.TamamlananServisBildiriminiOlusturAsync(_db, service.Id);
        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "Servis kaydı güncellendi.");
    }

    [HttpPost("admin/services/{serviceId:int}/delete")]
    public async Task<ActionResult<AdminSyncResponse>> DeleteAdminService(int serviceId, DeleteAdminEntityRequest request)
    {
        var service = await _db.ServisKayitlari.FindAsync(serviceId);
        if (service == null)
        {
            return NotFound(new ApiMessageResponse("Servis kaydı bulunamadı."));
        }

        _db.ServisKayitlari.Remove(service);
        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "Servis kaydı silindi.");
    }

    [HttpPost("admin/actions")]
    public async Task<ActionResult<AdminSyncResponse>> CreateAdminAction(CreateAdminActionRequest request)
    {
        var price = CalculateActionPrice(request.Price, request.MinPrice, request.MaxPrice);
        var action = new Islem
        {
            Ad = request.Name.Trim(),
            Kategori = CleanActionCategory(request.Category),
            MinimumFiyat = decimal.Round((decimal)request.MinPrice, 2),
            MaksimumFiyat = decimal.Round((decimal)request.MaxPrice, 2),
            Fiyat = price,
            Aciklama = request.Description?.Trim()
        };

        _db.Islemler.Add(action);
        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "Yeni işlem eklendi.");
    }

    [HttpPut("admin/actions/{actionId:int}")]
    public async Task<ActionResult<AdminSyncResponse>> UpdateAdminAction(int actionId, UpdateAdminActionRequest request)
    {
        var action = await _db.Islemler.FindAsync(actionId);
        if (action == null)
        {
            return NotFound(new ApiMessageResponse("İşlem bulunamadı."));
        }

        action.Ad = request.Name.Trim();
        action.Kategori = CleanActionCategory(request.Category);
        action.MinimumFiyat = decimal.Round((decimal)request.MinPrice, 2);
        action.MaksimumFiyat = decimal.Round((decimal)request.MaxPrice, 2);
        action.Fiyat = CalculateActionPrice(request.Price, request.MinPrice, request.MaxPrice);
        action.Aciklama = request.Description?.Trim();
        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "İşlem güncellendi.");
    }

    [HttpPost("admin/actions/{actionId:int}/delete")]
    public async Task<ActionResult<AdminSyncResponse>> DeleteAdminAction(int actionId, DeleteAdminEntityRequest request)
    {
        var actionInUse = await _db.ServisIslemler.AnyAsync(x => x.IslemId == actionId);
        if (actionInUse)
        {
            return BadRequest(new ApiMessageResponse("Bu işlem servis kayıtlarında kullanıldığı için silinemedi."));
        }

        var action = await _db.Islemler.FindAsync(actionId);
        if (action == null)
        {
            return NotFound(new ApiMessageResponse("İşlem bulunamadı."));
        }

        _db.Islemler.Remove(action);
        await _db.SaveChangesAsync();

        return await BuildAdminSyncAsync(request.AdminUsername.Trim(), "İşlem silindi.");
    }

    private async Task<ServisKaydi?> GetMemberServiceAsync(int memberId, int serviceId)
    {
        return await _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .FirstOrDefaultAsync(x => x.Id == serviceId && x.Cihaz != null && x.Cihaz.MusteriId == memberId);
    }

    private async Task<MemberSyncResponse> BuildMemberSyncAsync(int memberId, string message)
    {
        var member = await _db.Musteriler.FirstOrDefaultAsync(x => x.Id == memberId && x.UyeHesabiVar);
        if (member == null)
        {
            throw new InvalidOperationException("Member not found.");
        }

        var devices = await _db.Cihazlar
            .Where(x => x.MusteriId == memberId)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        var services = await _db.ServisKayitlari
            .Include(x => x.Cihaz)
            .Include(x => x.ServisIslemler)
            .Where(x => x.Cihaz != null && x.Cihaz.MusteriId == memberId)
            .OrderByDescending(x => x.Tarih)
            .ToListAsync();

        var actions = await _db.Islemler.OrderBy(x => x.Ad).ToListAsync();
        var notifications = await _db.MusteriBildirimleri
            .Include(x => x.ServisKaydi)
            .ThenInclude(x => x.Cihaz)
            .Where(x => x.MusteriId == memberId)
            .OrderByDescending(x => x.OlusturmaTarihi)
            .ToListAsync();
        var reviews = await _db.UyeYorumlari
            .Where(x => x.MusteriId == memberId)
            .OrderByDescending(x => x.OlusturmaTarihi)
            .ToListAsync();

        return new MemberSyncResponse(
            message,
            ToMemberDto(member),
            devices.Select(ToDeviceDto).ToList(),
            actions.Select(ToActionDto).ToList(),
            services.Select(ToServiceDto).ToList(),
            notifications.Select(ToNotificationDto).ToList(),
            reviews.Select(ToReviewDto).ToList());
    }

    private async Task<AdminSyncResponse> BuildAdminSyncAsync(string adminUsername, string message)
    {
        var members = await _db.Musteriler
            .OrderByDescending(x => x.KayitTarihi)
            .ToListAsync();

        var devices = await _db.Cihazlar
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        var services = await _db.ServisKayitlari
            .Include(x => x.ServisIslemler)
            .OrderByDescending(x => x.Tarih)
            .ToListAsync();

        var actions = await _db.Islemler.OrderBy(x => x.Ad).ToListAsync();
        var reviews = await _db.UyeYorumlari
            .Include(x => x.Musteri)
            .OrderByDescending(x => x.OlusturmaTarihi)
            .ToListAsync();

        return new AdminSyncResponse(
            message,
            adminUsername,
            members.Select(ToMemberDto).ToList(),
            devices.Select(ToDeviceDto).ToList(),
            actions.Select(ToActionDto).ToList(),
            services.Select(ToServiceDto).ToList(),
            reviews.Select(ToReviewDto).ToList());
    }

    private static MemberDto ToMemberDto(Musteri member)
    {
        return new MemberDto(
            member.Id,
            member.AdSoyad,
            member.Telefon,
            member.Email,
            member.Sifre,
            member.UyeHesabiVar,
            member.KayitTarihi,
            member.SonGirisTarihi,
            member.SifreSifirlamaKodu,
            member.SifreSifirlamaKodSonTarih);
    }

    private static DeviceDto ToDeviceDto(Cihaz device)
    {
        return new DeviceDto(
            device.Id,
            device.MusteriId,
            device.Marka,
            device.Model,
            device.ArizaAciklama);
    }

    private static ServiceActionDto ToActionDto(Islem action)
    {
        return new ServiceActionDto(
            action.Id,
            action.Ad,
            decimal.ToDouble(action.Fiyat),
            action.Kategori,
            decimal.ToDouble(action.MinimumFiyat),
            decimal.ToDouble(action.MaksimumFiyat),
            action.Aciklama ?? string.Empty);
    }

    private static string CleanActionCategory(string? category)
    {
        return string.IsNullOrWhiteSpace(category) ? "Genel" : category.Trim();
    }

    private static decimal CalculateActionPrice(double price, double minPrice, double maxPrice)
    {
        if (price > 0)
        {
            return decimal.Round((decimal)price, 2);
        }

        if (minPrice > 0 && maxPrice > 0)
        {
            return decimal.Round(((decimal)minPrice + (decimal)maxPrice) / 2m, 2);
        }

        return 0m;
    }

    private static ServiceRecordDto ToServiceDto(ServisKaydi service)
    {
        return new ServiceRecordDto(
            service.Id,
            service.CihazId,
            service.Tarih,
            service.Durum,
            decimal.ToDouble(service.ToplamFiyat),
            service.ServisIslemler.Select(x => x.IslemId).ToList(),
            service.FiyatOnayDurumu,
            service.FiyatOnayTarihi,
            service.FiyatCevapTarihi);
    }

    private static NotificationDto ToNotificationDto(MusteriBildirim notification)
    {
        var device = notification.ServisKaydi?.Cihaz;
        var deviceLabel = device == null ? string.Empty : $"{device.Marka} {device.Model}";

        return new NotificationDto(
            notification.Id,
            notification.MusteriId,
            notification.ServisKaydiId,
            notification.Tur,
            notification.Baslik,
            notification.Mesaj,
            notification.Okundu,
            notification.OlusturmaTarihi,
            notification.OkunmaTarihi,
            deviceLabel);
    }

    private static ReviewDto ToReviewDto(UyeYorum review)
    {
        return new ReviewDto(
            review.Id,
            review.MusteriId,
            review.Musteri?.AdSoyad ?? string.Empty,
            review.Baslik,
            review.Mesaj,
            review.Puan,
            review.YoneticiGordu,
            review.OlusturmaTarihi);
    }

    public sealed record MemberLoginRequest(string Email, string Password);
    public sealed record MemberRegisterRequest(string FullName, string Phone, string Email, string Password);
    public sealed record PasswordResetCodeRequest(string Email);
    public sealed record PasswordResetRequest(string Email, string Code, string NewPassword);
    public sealed record UpdateMemberProfileRequest(string FullName, string Phone, string Email, string? NewPassword);
    public sealed record CreateMemberServiceRequestRequest(string Brand, string Model, string IssueDescription);
    public sealed record CreateMemberReviewRequest(string Title, string Message, int Rating);
    public sealed record AdminLoginRequest(string Username, string Password);
    public sealed record ChangeAdminPasswordRequest(string AdminUsername, string CurrentPassword, string? NewPassword = null, string? NewUsername = null);
    public sealed record CreateAdminMemberRequest(string AdminUsername, string FullName, string Phone, string Email, bool CreateAccount);
    public sealed record CreateAdminDeviceRequest(string AdminUsername, int MemberId, string Brand, string Model, string IssueDescription);
    public sealed record CreateAdminServiceRequest(string AdminUsername, int DeviceId, string Status, List<int> ActionIds, bool SendPriceApproval = false);
    public sealed record UpdateAdminServiceRequest(string AdminUsername, int DeviceId, DateTime Date, string Status, List<int> ActionIds, bool SendPriceApproval = false);
    public sealed record CreateAdminActionRequest(string AdminUsername, string Name, double Price, string? Category = null, double MinPrice = 0, double MaxPrice = 0, string? Description = null);
    public sealed record UpdateAdminActionRequest(string AdminUsername, string Name, double Price, string? Category = null, double MinPrice = 0, double MaxPrice = 0, string? Description = null);
    public sealed record DeleteAdminEntityRequest(string AdminUsername);

    public sealed record ApiMessageResponse(string Message);
    public sealed record MemberSyncResponse(string Message, MemberDto Member, List<DeviceDto> Devices, List<ServiceActionDto> Actions, List<ServiceRecordDto> Services, List<NotificationDto> Notifications, List<ReviewDto> Reviews);
    public sealed record AdminSyncResponse(string Message, string AdminUsername, List<MemberDto> Members, List<DeviceDto> Devices, List<ServiceActionDto> Actions, List<ServiceRecordDto> Services, List<ReviewDto> Reviews);
    public sealed record MemberDto(int Id, string FullName, string Phone, string Email, string Password, bool HasAccount, DateTime RegisteredAt, DateTime? LastLoginAt, string? ResetCode, DateTime? ResetCodeExpiresAt);
    public sealed record DeviceDto(int Id, int MemberId, string Brand, string Model, string IssueDescription);
    public sealed record ServiceActionDto(int Id, string Name, double Price, string Category, double MinPrice, double MaxPrice, string Description);
    public sealed record ServiceRecordDto(int Id, int DeviceId, DateTime Date, string Status, double TotalPrice, List<int> ActionIds, string PriceApprovalStatus, DateTime? PriceApprovalSentAt, DateTime? PriceApprovalAnsweredAt);
    public sealed record NotificationDto(int Id, int MemberId, int? ServiceId, string Type, string Title, string Message, bool IsRead, DateTime CreatedAt, DateTime? ReadAt, string DeviceLabel);
    public sealed record ReviewDto(int Id, int MemberId, string MemberName, string Title, string Message, int Rating, bool SeenByAdmin, DateTime CreatedAt);
}
