using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IGetListAuctionRoundRepository;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.GetListAuctionRound.Querries
{
    public class GetListAuctionRoundHandler
        : IRequestHandler<GetListAuctionRoundRequest, GetListAuctionRoundResponse>
    {
        private readonly IGetListAuctionRoundRepository _getListAuctionRoundRepository;

        public GetListAuctionRoundHandler(
            IGetListAuctionRoundRepository getListAuctionRoundRepository
        )
        {
            _getListAuctionRoundRepository =
                getListAuctionRoundRepository
                ?? throw new ArgumentNullException(nameof(getListAuctionRoundRepository));
        }

        public async Task<GetListAuctionRoundResponse> Handle(
            GetListAuctionRoundRequest request,
            CancellationToken cancellationToken
        )
        {
            var auctionRounds =
                await _getListAuctionRoundRepository.GetAuctionRoundsByAuctionIdAsync(
                    request.AuctionId
                );

            var response = new GetListAuctionRoundResponse { AuctionRounds = auctionRounds };

            return response;
        }
    }
}
