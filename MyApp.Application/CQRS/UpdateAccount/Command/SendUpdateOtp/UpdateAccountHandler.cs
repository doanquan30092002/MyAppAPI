using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;

namespace MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp
{
    public class UpdateAccountHandler : IRequestHandler<UpdateAccountRequest, bool>
    {
        private readonly IUpdateAccountRepository _updateAccountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOTPService_1 _otpService;
        private readonly IMemoryCache _cache;

        public UpdateAccountHandler(
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

        public async Task<bool> Handle(
            UpdateAccountRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            var email = await _updateAccountRepository.GetEmailByUserIdAsync(userId);
            _cache.Set($"update_pending_{email}", request, TimeSpan.FromMinutes(10));
            var response = await _otpService.SendOtpAsync(email);
            return response;
        }
    }
}
