using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.ForgotPassword.Enums;

namespace MyApp.Application.CQRS.ForgotPassword.Service
{
    public interface IOTPService
    {
        OTPChannel Channel { get; }

        Task<string> SendOtpAsync(string to, string messageTemplate = null);

        Task<string> VerifyOtpAsync(string to, string code);

        bool VerifyResetGuid(string to, string guid);
    }
}
