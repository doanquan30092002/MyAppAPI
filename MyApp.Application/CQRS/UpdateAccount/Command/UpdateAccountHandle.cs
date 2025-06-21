using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.UpdateAccountRepository;

namespace MyApp.Application.CQRS.UpdateAccountAndProfile.Command
{
    public class UpdateAccountHandle : IRequestHandler<UpdateAccountRequest, UpdateAccountResponse>
    {
        private readonly IUpdateAccountRepository _updateAccountAndProfileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateAccountHandle(
            IUpdateAccountRepository updateAccountAndProfileRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _updateAccountAndProfileRepository = updateAccountAndProfileRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UpdateAccountResponse> Handle(
            UpdateAccountRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            var response = await _updateAccountAndProfileRepository.UpdateAccountInfo(
                userId,
                request.Email,
                request.PasswordOld,
                request.PasswordNew,
                request.PhoneNumber
            );
            return response;
        }
    }
}
