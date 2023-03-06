using AspNetCoreIdentityApp.Core.OptionsModels;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AspNetCoreIdentityApp.Core.Services
{
    public class EmailService : IEmailService
    {
        //private readonly IOptions<EmailSettings> _options;

        //public EmailService(IOptions<EmailSettings> options)
        //{
        //    _options = options; OPTIONS.VALUE EMAIL SETTINGS DÖNDÜĞÜ İÇİN AŞAĞIDAKİ GİBİ DÜZENLEDİM.
        //}

        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value; // email settings ten bir nesne üret demem lazım --> program.cs
        }

        public async Task SendResetPasswordEmail(string resetPasswordEmailLink, string toEmail)
        {
            var smtpClient = new SmtpClient();
            smtpClient.Host = _emailSettings.Host;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;  // Kendi credentialım olacak.
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.Email);
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = "Localhost | Reset password link";
            mailMessage.Body = @$"
                <h4>Please clink the below link to reset your password.</h4>
                <p><a href='{resetPasswordEmailLink}'>Reset Password Link</a></p>";
            mailMessage.IsBodyHtml = true;

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
