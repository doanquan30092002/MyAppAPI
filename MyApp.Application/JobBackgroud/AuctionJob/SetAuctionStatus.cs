using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.IAuctionRepository;

namespace MyApp.Application.JobBackgroud.AuctionJob
{
    public class SetAuctionStatus
    {
        private readonly IAuctionRepository _AuctionRepository;
        private readonly ILogger<SetAuctionStatus> _logger;

        public SetAuctionStatus(
            IAuctionRepository auctionRepository,
            ILogger<SetAuctionStatus> logger
        )
        {
            _AuctionRepository = auctionRepository;
            _logger = logger;
        }

        [AutomaticRetry(
            Attempts = 2,
            DelaysInSeconds = new[] { 60, 300 },
            OnAttemptsExceeded = AttemptsExceededAction.Fail
        )]
        public async Task SetAuctionStatusAsync(Guid auctionId, int newStatus, int oldStatus)
        {
            _logger.LogInformation(
                "Hangfire Job Start: SetAuctionStatus for AuctionId={AuctionId} at {Time}",
                auctionId,
                DateTime.Now
            );

            var auction = await _AuctionRepository.FindAuctionByIdAsync(auctionId);

            if (auction == null)
            {
                _logger.LogWarning(
                    "Auction with ID {AuctionId} not found at {Time}",
                    auctionId,
                    DateTime.Now
                );
                return;
            }

            if (auction.Status != oldStatus)
            {
                _logger.LogInformation(
                    "Auction with ID {AuctionId} has status {CurrentStatus}, expected {OldStatus}. Skipping update at {Time}",
                    auctionId,
                    auction.Status,
                    oldStatus,
                    DateTime.Now
                );
                return;
            }

            await _AuctionRepository.UpdateStatusAsync(auctionId, newStatus);

            _logger.LogInformation(
                "Hangfire Job Complete: SetAuctionStatus updated AuctionId={AuctionId} to Status={NewStatus} at {Time}",
                auctionId,
                newStatus,
                DateTime.Now
            );
        }
    }
}
