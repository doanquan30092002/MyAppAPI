using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.ForgotPassword.Enums;
using MyApp.Application.CQRS.ForgotPassword.Service;

namespace MyApp.Application.CQRS.ForgotPassword.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
    {
        private readonly IDictionary<OTPChannel, IOTPService> _otpServices;

        public ForgotPasswordCommandHandler(IEnumerable<IOTPService> otpServices)
        {
            _otpServices = otpServices.ToDictionary(s => s.Channel);
        }

        public async Task<string> Handle(
            ForgotPasswordCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!_otpServices.TryGetValue(request.Channel, out var otpService))
            {
                throw new NotSupportedException(
                    $"OTP channel '{request.Channel}' is not supported."
                );
            }

            var result = await otpService.SendOtpAsync(request.Contact);
            return result;
        }
    }
}
