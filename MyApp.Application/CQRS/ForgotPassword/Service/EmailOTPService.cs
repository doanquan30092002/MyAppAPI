using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using MyApp.Application.CQRS.ForgotPassword.Enums;
using MyApp.Core.Models;

namespace MyApp.Application.CQRS.ForgotPassword.Service
{
    public class EmailOTPService : IOTPService
    {
        private readonly IMemoryCache _cache;
        private readonly EmailSettings _emailSettings;
        private readonly TimeSpan _otpExpire = TimeSpan.FromMinutes(5);

        public EmailOTPService(IMemoryCache cache, EmailSettings emailSettings)
        {
            _cache = cache;
            _emailSettings = emailSettings;
        }

        public OTPChannel Channel => OTPChannel.Email;

        public async Task<string> SendOtpAsync(string to, string messageTemplate = null)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"otp_{to}", otp, _otpExpire);

            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = "Mã xác thực OTP",
                Body = messageTemplate ?? $"Mã OTP của bạn là: {otp}",
                IsBodyHtml = false,
            };
            message.To.Add(to);

            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
            {
                client.Credentials = new NetworkCredential(
                    _emailSettings.SenderEmail,
                    _emailSettings.Password
                );
                client.EnableSsl = true;
                await client.SendMailAsync(message);
            }

            return otp;
        }

        public Task<bool> VerifyOtpAsync(string to, string code)
        {
            if (
                _cache.TryGetValue($"otp_{to}", out var cachedObj)
                && cachedObj is string cachedOtp
                && cachedOtp == code
            )
            {
                _cache.Remove($"otp_{to}");
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
