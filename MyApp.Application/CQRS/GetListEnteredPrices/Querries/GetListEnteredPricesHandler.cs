using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.GetListEnteredPrices.Queries;
using MyApp.Application.CQRS.GetListEnteredPrices.Querries;
using MyApp.Application.Interfaces.IGetListEnteredPricesRepository;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.GetListEnteredPrices.Queries
{
    public class GetListEnteredPricesHandler
        : IRequestHandler<GetListEnteredPricesRequest, GetListEnteredPricesResponse>
    {
        private readonly IGetListEnteredPricesRepository _getListEnteredPricesRepository;

        public GetListEnteredPricesHandler(
            IGetListEnteredPricesRepository getListEnteredPricesRepository
        )
        {
            _getListEnteredPricesRepository = getListEnteredPricesRepository;
        }

        public async Task<GetListEnteredPricesResponse> Handle(
            GetListEnteredPricesRequest request,
            CancellationToken cancellationToken
        )
        {
            var auctionRoundPrices =
                await _getListEnteredPricesRepository.GetListEnteredPricesAsync(
                    request.AuctionRoundId
                );

            // Create response object
            var response = new GetListEnteredPricesResponse
            {
                ListAuctionRoundPrices =
                    auctionRoundPrices?.ToList() ?? new List<AuctionRoundPrices>(),
            };

            return response;
        }
    }
}
