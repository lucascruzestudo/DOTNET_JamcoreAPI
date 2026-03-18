using System.Net;
using System.Net.Mail;
using System.Reflection;
using Project.Domain.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using Resend;

namespace Project.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly SmtpClient _smtpClient;
    private readonly IResend _resend;
    private readonly ILogger<EmailService> _logger;
    private readonly bool _useResend;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger, IResend resend)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
        _resend = resend;
        _useResend = !string.IsNullOrEmpty(_emailSettings.ResendApiKey);

        _smtpClient = new SmtpClient(_emailSettings.Server)
        {
            Port = _emailSettings.Port,
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
            EnableSsl = _emailSettings.EnableSsl,
        };
    }

    public async Task SendEmailAsync(string to, string subject, string bodyContent)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .First(x => x.EndsWith("Template.html", StringComparison.OrdinalIgnoreCase));
            string template = await GetEmbeddedResourceContentAsync(resourceName);
            string body = template.Replace("{{bodyContent}}", bodyContent);

            if (_useResend)
            {
                await SendViaResendAsync(to, subject, body);
            }
            else
            {
                await SendViaSmtpAsync(to, subject, body);
            }

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (SmtpException ex)
        {
            _logger.LogWarning(ex, "SMTP error sending email to {To}. Subject: {Subject}", to, subject);
        }
        catch (SocketException ex)
        {
            _logger.LogWarning(ex, "Network error sending email to {To}. Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error sending email to {To}. Subject: {Subject}", to, subject);
        }
    }

    private async Task SendViaResendAsync(string to, string subject, string body)
    {
        var emailMessage = new EmailMessage()
        {
            From = _emailSettings.ResendFromEmail,
            To = to,
            Subject = "ProjectAPI - " + subject,
            HtmlBody = body,
        };

        await _resend.EmailSendAsync(emailMessage);
    }

    private async Task SendViaSmtpAsync(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.Username),
            Subject = "ProjectAPI - " + subject,
            Body = body,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(to);

        await _smtpClient.SendMailAsync(mailMessage);
    }

    private static async Task<string> GetEmbeddedResourceContentAsync(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"Resource '{resourceName}' not found.");
        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }
}