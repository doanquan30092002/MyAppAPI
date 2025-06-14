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
    public class SetAuctionUpdateableFalse
    {
        private readonly IAuctionRepository _AuctionRepository;
        private readonly ILogger<SetAuctionUpdateableFalse> _logger;

        public SetAuctionUpdateableFalse(
            IAuctionRepository auctionRepository,
            ILogger<SetAuctionUpdateableFalse> logger
        )
        {
            _AuctionRepository = auctionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Đặt trường Updateable của phiên đấu giá về false.
        /// </summary>
        /// <param name="auctionId">Id phiên đấu giá</param>
        [AutomaticRetry(
            Attempts = 2,
            DelaysInSeconds = new[] { 60, 300 },
            OnAttemptsExceeded = AttemptsExceededAction.Fail
        )]
        public async Task SetAuctionUpdateableFalseAsync(Guid auctionId)
        {
            _logger.LogInformation(
                "Hangfire Job Start: SetAuctionUpdateableFalseAsync for AuctionId={AuctionId} at {Time}",
                auctionId,
                DateTime.UtcNow
            );
            await _AuctionRepository.UpdateAuctionUpdateableAsync(auctionId, false);
            _logger.LogInformation(
                "Hangfire Job Complete: SetAuctionUpdateableFalseAsync for AuctionId={AuctionId} at {Time}",
                auctionId,
                DateTime.UtcNow
            );
        }
    }
}
