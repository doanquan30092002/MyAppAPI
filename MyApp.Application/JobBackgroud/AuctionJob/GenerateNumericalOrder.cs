using Hangfire;
using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction;

namespace MyApp.Application.JobBackgroud.AuctionJob
{
    public class GenerateNumericalOrder
    {
        private readonly IAssginAuctioneerAndPublicAuctionRepository _repository;
        private readonly ILogger<GenerateNumericalOrder> _logger;

        public GenerateNumericalOrder(
            IAssginAuctioneerAndPublicAuctionRepository repository,
            ILogger<GenerateNumericalOrder> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        [AutomaticRetry(
            Attempts = 2,
            DelaysInSeconds = new[] { 60, 300 },
            OnAttemptsExceeded = AttemptsExceededAction.Fail
        )]
        public async Task GenerateNumericalOrderAsync(Guid auctionId)
        {
            _logger.LogInformation(
                "Hangfire Job Start: GenerateNumbericalOrder for AuctionId={AuctionId} at {Time}",
                auctionId,
                DateTime.Now
            );
            // Logic to generate numerical order for the auction
            // This is a placeholder for the actual implementation
            await _repository.GenerateNumbericalOrderAsync(auctionId);
            _logger.LogInformation(
                "Hangfire Job Complete: GenerateNumbericalOrder for AuctionId={AuctionId} at {Time}",
                auctionId,
                DateTime.Now
            );
        }
    }
}
