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
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IOTPService _otpService;
        private readonly IForgetPasswordRepository _forgetPasswordRepository;

        public ResetPasswordHandler(
            IOTPService otpService,
            IForgetPasswordRepository forgetPasswordRepository
        )
        {
            _otpService = otpService;
            _forgetPasswordRepository = forgetPasswordRepository;
        }

        public async Task<bool> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken
        )
        {
            var isValidGuid = _otpService.VerifyResetGuid(request.Contact, request.ResetGuid);
            if (!isValidGuid)
            {
                throw new UnauthorizedAccessException("ResetGuid không hợp lệ hoặc đã hết hạn.");
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
