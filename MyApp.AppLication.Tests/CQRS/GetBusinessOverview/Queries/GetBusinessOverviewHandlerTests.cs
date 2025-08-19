using System;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetBusinessOverview.Queries;
using MyApp.Application.Interfaces.IGetBusinessOverviewRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetBusinessOverview.Queries.Tests
{
    public class FakeGetBusinessOverviewRepository : IGetBusinessOverviewRepository
    {
        private readonly GetBusinessOverviewResponse _response;
        private readonly bool _throwException;

        public FakeGetBusinessOverviewRepository(
            GetBusinessOverviewResponse response = null,
            bool throwException = false
        )
        {
            _response = response;
            _throwException = throwException;
        }

        public Task<GetBusinessOverviewResponse> GetBusinessOverview(
            GetBusinessOverviewRequest request
        )
        {
            if (_throwException)
                throw new Exception("Error while fetching business overview.");

            return Task.FromResult(_response);
        }
    }

    [TestFixture]
    public class GetBusinessOverviewHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnResponse_WhenDataExists()
        {
            // Arrange
            var request = new GetBusinessOverviewRequest
            {
                CategoryId = 1,
                AuctionStartDate = DateTime.Now.AddDays(-10),
                AuctionEndDate = DateTime.Now.AddDays(10),
            };

            var expectedResponse = new GetBusinessOverviewResponse
            {
                TotalAuctions = 5,
                TotalParticipants = 20,
                TotalSuccessfulAuctions = 3,
                TotalCancelledAuctions = 1,
            };

            var fakeRepo = new FakeGetBusinessOverviewRepository(expectedResponse);
            var handler = new GetBusinessOverviewHandler(fakeRepo);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.TotalAuctions, result.TotalAuctions);
            Assert.AreEqual(expectedResponse.TotalParticipants, result.TotalParticipants);
            Assert.AreEqual(
                expectedResponse.TotalSuccessfulAuctions,
                result.TotalSuccessfulAuctions
            );
            Assert.AreEqual(expectedResponse.TotalCancelledAuctions, result.TotalCancelledAuctions);
        }

        [Test]
        public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var request = new GetBusinessOverviewRequest
            {
                CategoryId = 2,
                AuctionStartDate = DateTime.Now.AddDays(-5),
                AuctionEndDate = DateTime.Now.AddDays(5),
            };

            var fakeRepo = new FakeGetBusinessOverviewRepository(null);
            var handler = new GetBusinessOverviewHandler(fakeRepo);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Handle_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            var request = new GetBusinessOverviewRequest
            {
                CategoryId = 3,
                AuctionStartDate = DateTime.Now.AddDays(-1),
                AuctionEndDate = DateTime.Now.AddDays(1),
            };

            var fakeRepo = new FakeGetBusinessOverviewRepository(null, throwException: true);
            var handler = new GetBusinessOverviewHandler(fakeRepo);

            // Act + Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await handler.Handle(request, CancellationToken.None)
            );
        }
    }
}
