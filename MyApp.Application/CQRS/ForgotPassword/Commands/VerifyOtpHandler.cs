using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.ForgotPassword.Service;
using MyApp.Application.Interfaces.IForgetPasswordRepository;

namespace MyApp.Application.CQRS.ForgotPassword.Commands
{
    public class VerifyOtpHandler : IRequestHandler<VerifyOtpCommand, string>
    {
        private readonly IOTPService _otpService;

        public VerifyOtpHandler(
            IOTPService otpService,
            IForgetPasswordRepository forgetPasswordRepository
        )
        {
            _otpService = otpService;
        }

        public async Task<string> Handle(
            VerifyOtpCommand request,
            CancellationToken cancellationToken
        )
        {
            var resetGuid = await _otpService.VerifyOtpAsync(request.Contact, request.OtpCode);
            return resetGuid;
        }
    }
}
