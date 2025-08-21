using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.ForgotPassword.Enums;
using MyApp.Application.CQRS.ForgotPassword.Service;
using MyApp.Application.Interfaces.IForgetPasswordRepository;

namespace MyApp.Application.CQRS.ForgotPassword.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
    {
        private readonly IDictionary<OTPChannel, IOTPService> _otpServices;
        private readonly IForgetPasswordRepository _forgetPasswordRepository;

        public ForgotPasswordCommandHandler(
            IEnumerable<IOTPService> otpServices,
            IForgetPasswordRepository forgetPasswordRepository
        )
        {
            _otpServices = otpServices.ToDictionary(s => s.Channel);
            _forgetPasswordRepository = forgetPasswordRepository;
        }

        public async Task<string> Handle(
            ForgotPasswordCommand request,
            CancellationToken cancellationToken
        )
        {
            var account = await _forgetPasswordRepository.FindByContactAsync(
                request.Contact,
                request.Channel
            );
            if (account == null)
            {
                throw new ArgumentException("Tài khoản không tồn tại với thông tin liên hệ này.");
            }

            if (!_otpServices.TryGetValue(request.Channel, out var otpService))
            {
                throw new NotSupportedException($"Kênh gửi OTP '{request.Channel}' không hỗ trợ.");
            }

            var result = await otpService.SendOtpAsync(request.Contact);
            return result;
        }
    }
}
