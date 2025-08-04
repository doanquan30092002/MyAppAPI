using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IAuctionRepository;

namespace MyApp.Application.CQRS.Auction.MarkAuctionAsSuccessful.Command
{
    public class MarkAuctionAsSuccessfulHandler
        : IRequestHandler<MarkAuctionAsSuccessfulCommand, bool>
    {
        private readonly IAuctionRepository _auctionRepository;

        public MarkAuctionAsSuccessfulHandler(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task<bool> Handle(
            MarkAuctionAsSuccessfulCommand request,
            CancellationToken cancellationToken
        )
        {
            return await _auctionRepository.MarkAuctionAsSuccessfulAsync(request.AuctionId);
        }
    }
}
