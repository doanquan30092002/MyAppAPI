using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyApp.Application.CQRS.ForgotPassword.Commands;
using MyApp.Application.CQRS.ForgotPassword.Enums;

namespace MyApp.Application.CQRS.ForgotPassword.Service
{
    public class SmsOTPService : IOTPService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public SmsOTPService(IConfiguration config, HttpClient http)
        {
            _config = config;
            _http = new HttpClient();
        }

        public OTPChannel Channel => OTPChannel.SMS;

        public async Task<string> SendOtpAsync(string phone, string messageTemplate = null)
        {
            var apiKey = _config["Esms:ApiKey"];
            var secretKey = _config["Esms:SecretKey"];
            var brandname = _config["Esms:Brandname"];
            var url =
                $"https://rest.esms.vn/MainService.svc/json/SendMessageAutoGenCode_V4_get"
                + $"?Phone={phone}&ApiKey={apiKey}&SecretKey={secretKey}&TimeAlive=2&NumCharOfCode=6&Brandname={brandname}&Type=2&message={{OTP}} la ma xac minh dang ky {brandname} cua ban";

            var res = await _http.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<EsmsResponse>(content);

            if (data.CodeResult != "100")
                throw new Exception("OTP gửi thất bại: " + data.ErrorMessage);

            return data.SMSID ?? "OTP sent successfully.";
        }

        public async Task<bool> VerifyOtpAsync(string phone, string code)
        {
            var apiKey = _config["Esms:ApiKey"];
            var secretKey = _config["Esms:SecretKey"];
            var url =
                $"https://rest.esms.vn/MainService.svc/json/CheckCodeGen_V4_get?ApiKey={apiKey}&SecretKey={secretKey}&Phone={phone}&Code={code}";

            var res = await _http.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<EsmsResponse>(content);

            return data.CodeResult == "100";
        }
    }
}
