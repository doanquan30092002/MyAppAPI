using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Core.DTOs.AuctionAssetsDTO;

namespace MyApp.Application.CQRS.AuctionAssetsV2.GetAuctionAssetsHighestBid.Queries
{
    public class GetAssetsHighestBidHandler
        : IRequestHandler<GetAssetsHighestBidRequest, List<AuctionAssetsWithHighestBidResponse>>
    {
        private readonly IAuctionAssetsRepository _auctionAssetsRepository;

        public GetAssetsHighestBidHandler(IAuctionAssetsRepository auctionAssetsRepository)
        {
            _auctionAssetsRepository = auctionAssetsRepository;
        }

        public async Task<List<AuctionAssetsWithHighestBidResponse>> Handle(
            GetAssetsHighestBidRequest request,
            CancellationToken cancellationToken
        )
        {
            var result =
                await _auctionAssetsRepository.GetAuctionAssetsWithHighestBidByAuctionIdAsync(
                    request.AuctionId
                );

            return result;
        }
    }
}
