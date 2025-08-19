using System;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;
using MyApp.Application.Interfaces.IGetAuctionRoundStatisticsRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries.Tests
{
    public class FakeGetAuctionRoundStatisticsRepository : IGetAuctionRoundStatisticsRepository
    {
        private readonly Func<
            GetAuctionRoundStatisticsRequest,
            Task<GetAuctionRoundStatisticsResponse?>
        > _handler;

        public FakeGetAuctionRoundStatisticsRepository(
            Func<GetAuctionRoundStatisticsRequest, Task<GetAuctionRoundStatisticsResponse?>> handler
        )
        {
            _handler = handler;
        }

        public Task<GetAuctionRoundStatisticsResponse> GetAuctionRoundStatistics(
            GetAuctionRoundStatisticsRequest request
        )
        {
            return _handler(request)!;
        }
    }

    [TestFixture]
    public class GetAuctionRoundStatisticsHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnStatistics_WhenValidAuction()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var fakeRepo = new FakeGetAuctionRoundStatisticsRepository(req =>
            {
                return Task.FromResult<GetAuctionRoundStatisticsResponse?>(
                    new GetAuctionRoundStatisticsResponse
                    {
                        TotalParticipants = 5,
                        TotalAssets = 3,
                        TotalBids = 10,
                    }
                );
            });

            var handler = new GetAuctionRoundStatisticsHandler(fakeRepo);
            var request = new GetAuctionRoundStatisticsRequest { AuctionId = auctionId };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(5, result.TotalParticipants);
            Assert.AreEqual(3, result.TotalAssets);
            Assert.AreEqual(10, result.TotalBids);
        }

        [Test]
        public async Task Handle_ShouldReturnNull_WhenRepoReturnsNull()
        {
            // Arrange
            var fakeRepo = new FakeGetAuctionRoundStatisticsRepository(req =>
            {
                return Task.FromResult<GetAuctionRoundStatisticsResponse?>(null);
            });

            var handler = new GetAuctionRoundStatisticsHandler(fakeRepo);
            var request = new GetAuctionRoundStatisticsRequest { AuctionId = Guid.NewGuid() };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Handle_ShouldThrowArgumentException_WhenAuctionIdIsEmpty()
        {
            // Arrange
            var fakeRepo = new FakeGetAuctionRoundStatisticsRepository(req =>
            {
                if (req.AuctionId == Guid.Empty)
                    throw new ArgumentException("AuctionId không hợp lệ");

                return Task.FromResult<GetAuctionRoundStatisticsResponse?>(
                    new GetAuctionRoundStatisticsResponse()
                );
            });

            var handler = new GetAuctionRoundStatisticsHandler(fakeRepo);
            var request = new GetAuctionRoundStatisticsRequest { AuctionId = Guid.Empty };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(request, CancellationToken.None)
            );
        }
    }
}
