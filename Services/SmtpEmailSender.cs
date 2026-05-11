using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using Microsoft.Extensions.Options;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public SmtpEmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var userName = _options.UserName.Trim();
        var password = NormalizePassword(_options.Password);
        var fromEmail = string.IsNullOrWhiteSpace(_options.FromEmail)
            ? userName
            : _options.FromEmail.Trim();

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException("SMTP ayarlari eksik. appsettings.json icindeki Smtp UserName, Password ve FromEmail alanlarini doldurun.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = Math.Max(_options.TimeoutSeconds, 5) * 1000,
            Credentials = new NetworkCredential(userName, password)
        };

        try
        {
            var sendTask = client.SendMailAsync(message);
            var completedTask = await Task.WhenAny(sendTask, Task.Delay(client.Timeout));

            if (completedTask != sendTask)
            {
                throw new TimeoutException($"SMTP baglantisi {Math.Max(_options.TimeoutSeconds, 5)} saniye icinde cevap vermedi. Bu cihazdaki ag SMTP portlarini engelliyor olabilir.");
            }

            await sendTask;
        }
        catch (SmtpException ex) when (ex.InnerException is SocketException socketEx)
        {
            throw new InvalidOperationException($"SMTP baglantisi kurulamadi: {socketEx.Message}", ex);
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    private static string NormalizePassword(string password)
    {
        return string.Concat(password.Where(ch => char.IsWhiteSpace(ch) == false));
    }
}
