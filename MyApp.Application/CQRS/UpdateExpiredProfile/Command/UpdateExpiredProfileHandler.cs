using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.UpdateExpiredProfile;

namespace MyApp.Application.CQRS.UpdateExpiredProfile.Command
{
    public class UpdateExpiredProfileHandler
        : IRequestHandler<UpdateExpiredProfileRequest, UpdateExpiredProfileResponse>
    {
        private readonly IUpdateExpiredProfileRepository _updateExpiredProfileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateExpiredProfileHandler(
            IUpdateExpiredProfileRepository updateExpiredProfileRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _updateExpiredProfileRepository = updateExpiredProfileRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UpdateExpiredProfileResponse> Handle(
            UpdateExpiredProfileRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            var response = await _updateExpiredProfileRepository.UpdateExpiredProfileAsync(
                userId,
                request
            );

            return response;
        }
    }
}
