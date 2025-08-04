using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IAuctionRepository;

namespace MyApp.Application.CQRS.Auction.RejectAuction
{
    public class RejectAuctionHandler : IRequestHandler<RejectAuction, bool>
    {
        private readonly IAuctionRepository _auctionRepository;

        public RejectAuctionHandler(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task<bool> Handle(RejectAuction request, CancellationToken cancellationToken)
        {
            return await _auctionRepository.RejectAuctionAsync(
                request.AuctionId,
                request.RejectReason
            );
        }
    }
}
