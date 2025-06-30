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
    public class SetAuctionUpdateable
    {
        private readonly IAuctionRepository _AuctionRepository;
        private readonly ILogger<SetAuctionUpdateable> _logger;

        public SetAuctionUpdateable(
            IAuctionRepository auctionRepository,
            ILogger<SetAuctionUpdateable> logger
        )
        {
            _AuctionRepository = auctionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Đặt trường Updateable của phiên đấu giá về true hoặc false.
        /// </summary>
        /// <param name="auctionId">Id phiên đấu giá</param>
        [AutomaticRetry(
            Attempts = 2,
            DelaysInSeconds = new[] { 60, 300 },
            OnAttemptsExceeded = AttemptsExceededAction.Fail
        )]
        public async Task SetAuctionUpdateableAsync(Guid auctionId, bool status)
        {
            _logger.LogInformation(
                $"Hangfire Job Start: SetAuctionUpdateableAsync for AuctionId={auctionId} at {DateTime.Now}",
                auctionId,
                DateTime.Now
            );
            await _AuctionRepository.UpdateAuctionUpdateableAsync(auctionId, status);
            _logger.LogInformation(
                $"Hangfire Job Complete: SetAuctionUpdateableAsync for AuctionId={auctionId} at {DateTime.Now}",
                auctionId,
                DateTime.Now
            );
        }
    }
}
