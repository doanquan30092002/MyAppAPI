using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.ForgotPassword.Enums;

namespace MyApp.Application.CQRS.ForgotPassword.Commands
{
    public class VerifyOtpAndChangePasswordCommand : IRequest<bool>
    {
        public string Contact { get; set; }
        public string OtpCode { get; set; }
        public string NewPassword { get; set; }
        public OTPChannel Channel { get; set; }
    }
}
