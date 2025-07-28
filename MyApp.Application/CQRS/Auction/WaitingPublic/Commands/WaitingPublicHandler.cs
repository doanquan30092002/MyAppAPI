using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.JobBackgroud.AuctionJob;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.Auction.WaitingPublic.Commands
{
    public class WaitingPublicHandler : IRequestHandler<WaitingPublicCommand, bool>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public WaitingPublicHandler(
            IAuctionRepository auctionRepository,
            IBackgroundJobClient backgroundJobClient
        )
        {
            _auctionRepository = auctionRepository;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<bool> Handle(
            WaitingPublicCommand request,
            CancellationToken cancellationToken
        )
        {
            var success = await _auctionRepository.WaitingPublicAsync(request.AuctionId);

            if (!success)
                return false;

            var auction = await _auctionRepository.FindAuctionByIdAsync(request.AuctionId);
            if (auction == null || auction.RegisterOpenDate <= DateTime.Now)
                return false;

            _backgroundJobClient.Schedule<SetAuctionStatus>(
                job => job.SetAuctionStatusAsync(request.AuctionId, 5, 4),
                auction.RegisterOpenDate
            );

            return true;
        }
    }
}
