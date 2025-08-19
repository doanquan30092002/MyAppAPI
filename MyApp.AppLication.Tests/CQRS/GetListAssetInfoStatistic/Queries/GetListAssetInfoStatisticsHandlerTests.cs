using System;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetListAssetInfoStatistic.Queries;
using MyApp.Application.Interfaces.IGetListAssetInfostatisticsRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetListAssetInfoStatistic.Queries.Tests
{
    // Fake Repository để test mà không dùng Moq
    public class FakeGetListAssetInfoStatisticsRepository : IGetListAssetInfostatisticsRepository
    {
        private readonly GetListAssetInfoStatisticsResponse _response;
        private readonly bool _throwException;

        public FakeGetListAssetInfoStatisticsRepository(
            GetListAssetInfoStatisticsResponse response = null,
            bool throwException = false
        )
        {
            _response = response;
            _throwException = throwException;
        }

        public Task<GetListAssetInfoStatisticsResponse> GetAuctionAssetsStatistics(
            GetListAssetInfostatisticsRequest request
        )
        {
            if (_throwException)
                throw new Exception("Auction asset not found.");

            return Task.FromResult(_response);
        }
    }

    [TestFixture]
    public class GetListAssetInfoStatisticsHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnResponse_WhenDataExists()
        {
            // Arrange
            var request = new GetListAssetInfostatisticsRequest
            {
                AuctionAssetsId = Guid.NewGuid(),
            };

            var expectedResponse = new GetListAssetInfoStatisticsResponse
            {
                AssetName = "Car",
                StartingPrice = 1000,
                TotalBids = 5,
                HighestPrice = 2000,
                TotalParticipants = 3,
                HasWinner = true,
            };

            var fakeRepo = new FakeGetListAssetInfoStatisticsRepository(expectedResponse);
            var handler = new GetListAssetInfoStatisticsHandler(fakeRepo);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.AssetName, result.AssetName);
            Assert.AreEqual(expectedResponse.TotalBids, result.TotalBids);
            Assert.AreEqual(expectedResponse.HighestPrice, result.HighestPrice);
            Assert.AreEqual(expectedResponse.TotalParticipants, result.TotalParticipants);
            Assert.AreEqual(expectedResponse.HasWinner, result.HasWinner);
        }

        [Test]
        public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var request = new GetListAssetInfostatisticsRequest
            {
                AuctionAssetsId = Guid.NewGuid(),
            };

            var fakeRepo = new FakeGetListAssetInfoStatisticsRepository(null);
            var handler = new GetListAssetInfoStatisticsHandler(fakeRepo);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Handle_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            var request = new GetListAssetInfostatisticsRequest
            {
                AuctionAssetsId = Guid.NewGuid(),
            };

            var fakeRepo = new FakeGetListAssetInfoStatisticsRepository(null, throwException: true);
            var handler = new GetListAssetInfoStatisticsHandler(fakeRepo);

            // Act + Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await handler.Handle(request, CancellationToken.None)
            );
        }
    }
}
