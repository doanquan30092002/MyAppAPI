using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyApp.Core.Models;

namespace MyApp.Application.Common.Services.SendMessage
{
    public class EmailSendMessage : ISendMessage
    {
        public SendMessageChannel Channel => SendMessageChannel.Email;

        private readonly EmailSettings _settings;

        public EmailSendMessage(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> SendAsync(string to, string subject, string content)
        {
            try
            {
                using (
                    var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
                    {
                        EnableSsl = true,
                        Credentials = new NetworkCredential(
                            _settings.SenderEmail,
                            _settings.Password
                        ),
                    }
                )
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(_settings.SenderEmail, _settings.SenderName);
                    mail.To.Add(to);
                    mail.Subject = subject ?? "No Subject";
                    mail.Body = content;
                    mail.IsBodyHtml = true;

                    await client.SendMailAsync(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
