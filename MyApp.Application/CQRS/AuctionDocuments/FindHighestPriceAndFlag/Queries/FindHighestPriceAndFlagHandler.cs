using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.IFindHighestPriceAndFlag;

namespace MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries
{
    public class FindHighestPriceAndFlagHandler
        : IRequestHandler<FindHighestPriceAndFlagRequest, FindHighestPriceAndFlagResponse>
    {
        private readonly IFindHighestPriceAndFlag _findHighestPriceAndFlag;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FindHighestPriceAndFlagHandler(
            IFindHighestPriceAndFlag findHighestPriceAndFlag,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _findHighestPriceAndFlag = findHighestPriceAndFlag;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<FindHighestPriceAndFlagResponse> Handle(
            FindHighestPriceAndFlagRequest request,
            CancellationToken cancellationToken
        )
        {
            Guid? userId = null;
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?.Value;

            if (Guid.TryParse(userIdStr, out var parsedGuid))
            {
                userId = parsedGuid;
            }

            if (userId == null)
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var result = await _findHighestPriceAndFlag.FindHighestPriceAndFlag(
                request.AuctionId,
                userId.Value
            );

            return result;
        }
    }
}
