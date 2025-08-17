using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.IFindHighestPriceAndFlag;

namespace MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries
{
    public class FindHighestPriceAndFlagHandler
        : IRequestHandler<FindHighestPriceAndFlagRequest, FindHighestPriceAndFlagResponse>
    {
        private readonly IFindHighestPriceAndFlag _findHighestPriceAndFlag;
        private readonly ICurrentUserService _currentUserService;

        public FindHighestPriceAndFlagHandler(
            IFindHighestPriceAndFlag findHighestPriceAndFlag,
            ICurrentUserService currentUserService
        )
        {
            _findHighestPriceAndFlag = findHighestPriceAndFlag;
            _currentUserService = currentUserService;
        }

        public async Task<FindHighestPriceAndFlagResponse> Handle(
            FindHighestPriceAndFlagRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var result = await _findHighestPriceAndFlag.FindHighestPriceAndFlag(
                request.AuctionId,
                userId
            );

            return result;
        }
    }
}
