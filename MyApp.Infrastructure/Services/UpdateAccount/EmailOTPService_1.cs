using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.UpdateAccount.Service;
using MyApp.Core.Models;

namespace MyApp.Infrastructure.Services.UpdateAccount
{
    public class EmailOTPService_1 : IOTPService_1
    {
        private readonly IMemoryCache _cache;
        private readonly EmailSettings _emailSettings;
        private readonly TimeSpan _otpExpire = TimeSpan.FromMinutes(5);

        public EmailOTPService_1(IMemoryCache cache, IOptions<EmailSettings> emailOptions)
        {
            _cache = cache;
            _emailSettings = emailOptions.Value;
        }

        public async Task<bool> SendOtpAsync(string to)
        {
            try
            {
                var otp = new Random().Next(100000, 999999).ToString();
                _cache.Set($"otp_{to}", otp, _otpExpire);

                var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = Message.EMAIL_SUBJECT,
                    Body = $"{Message.EMAIL_BODY}{otp}",
                    IsBodyHtml = false,
                };
                message.To.Add(to);

                using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(
                        _emailSettings.SenderEmail,
                        _emailSettings.Password
                    ),
                    EnableSsl = true,
                };

                await smtp.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây nếu cần
                return false;
            }
        }

        public Task<(bool Success, string Message)> VerifyOtpAsync(string to, string code)
        {
            if (_cache.TryGetValue($"otp_{to}", out string? cachedOtp))
            {
                if (cachedOtp == code)
                {
                    _cache.Remove($"otp_{to}");
                    return Task.FromResult((true, Message.OTP_CORRECT));
                }
                return Task.FromResult((false, Message.OTP_INCORRECT));
            }

            return Task.FromResult((false, Message.OTP_EXPIRED_OR_NOT_EXIST));
        }
    }
}
