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
    public class VerifyOtpAndChangePasswordHandler
        : IRequestHandler<VerifyOtpAndChangePasswordCommand, bool>
    {
        private readonly IOTPService _otpService;
        private readonly IForgetPasswordRepository _forgetPasswordRepository;

        public VerifyOtpAndChangePasswordHandler(
            IOTPService otpService,
            IForgetPasswordRepository forgetPasswordRepository
        )
        {
            _otpService = otpService;
            _forgetPasswordRepository = forgetPasswordRepository;
        }

        public async Task<bool> Handle(
            VerifyOtpAndChangePasswordCommand request,
            CancellationToken cancellationToken
        )
        {
            var isValidOtp = await _otpService.VerifyOtpAsync(request.Contact, request.OtpCode);
            if (!isValidOtp)
            {
                return false;
            }

            var result = await _forgetPasswordRepository.UpdatePasswordAsync(
                request.Contact,
                request.Channel,
                request.NewPassword
            );
            return result;
        }
    }
}
