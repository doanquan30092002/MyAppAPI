using MediatR;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;

namespace MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp
{
    public class UpdateAccountHandler : IRequestHandler<UpdateAccountRequest, bool>
    {
        private readonly IUpdateAccountRepository _updateAccountRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IOTPService_1 _otpService;
        private readonly IMemoryCache _cache;

        public UpdateAccountHandler(
            IUpdateAccountRepository updateAccountRepository,
            ICurrentUserService currentUserService,
            IOTPService_1 otpService,
            IMemoryCache cache
        )
        {
            _updateAccountRepository = updateAccountRepository;
            _currentUserService = currentUserService;
            _otpService = otpService;
            _cache = cache;
        }

        public async Task<bool> Handle(
            UpdateAccountRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Yêu cầu đăng nhập");
            var email = await _updateAccountRepository.GetEmailByUserIdAsync(userId);
            _cache.Set($"update_pending_{email}", request, TimeSpan.FromMinutes(10));
            var response = await _otpService.SendOtpAsync(email);
            return response;
        }
    }
}
