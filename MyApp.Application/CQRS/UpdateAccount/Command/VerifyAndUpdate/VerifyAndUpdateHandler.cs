using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;

namespace MyApp.Application.CQRS.UpdateAccount.Command.VerifyAndUpdate
{
    public class VerifyAndUpdateHandler
        : IRequestHandler<VerifyAndUpdateRequest, UpdateAccountResponse>
    {
        private readonly IUpdateAccountRepository _updateAccountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOTPService_1 _otpService;
        private readonly IMemoryCache _cache;

        public VerifyAndUpdateHandler(
            IUpdateAccountRepository updateAccountRepository,
            IHttpContextAccessor httpContextAccessor,
            IOTPService_1 otpService,
            IMemoryCache cache
        )
        {
            _updateAccountRepository = updateAccountRepository;
            _httpContextAccessor = httpContextAccessor;
            _otpService = otpService;
            _cache = cache;
        }

        public async Task<UpdateAccountResponse> Handle(
            VerifyAndUpdateRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            var email = await _updateAccountRepository.GetEmailByUserIdAsync(userId);

            var isVerify = await _otpService.VerifyOtpAsync(email, request.OtpCode);
            if (!isVerify.Success)
            {
                return new UpdateAccountResponse { Code = 400, Message = isVerify.Message };
            }
            if (
                !_cache.TryGetValue(
                    $"update_pending_{email}",
                    out UpdateAccountRequest updateAccountRequest
                )
            )
            {
                return new UpdateAccountResponse
                {
                    Code = 400,
                    Message = "Không tìm thấy dữ liệu cập nhật.",
                };
            }
            var response = await _updateAccountRepository.UpdateAccountInfo(
                userId,
                updateAccountRequest.Email,
                updateAccountRequest.PasswordOld,
                updateAccountRequest.PasswordNew,
                updateAccountRequest.PhoneNumber
            );
            _cache.Remove($"update_pending_{email}");
            return response;
        }
    }
}
