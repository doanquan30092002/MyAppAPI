using System;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetStatisticOverview.Queries;
using MyApp.Application.Interfaces.IGetStatisticOverviewRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetStatisticOverview.Queries.Tests
{
    // Fake repository thay cho Moq
    public class FakeGetStatisticOverviewRepository : IGetStatisticOverviewRepository
    {
        private readonly GetStatisticOverviewResponse _response;
        private readonly bool _throwException;

        public FakeGetStatisticOverviewRepository(
            GetStatisticOverviewResponse response = null,
            bool throwException = false
        )
        {
            _response = response;
            _throwException = throwException;
        }

        public Task<GetStatisticOverviewResponse> GetStatisticOverview(
            GetStatisticOverviewRequest request
        )
        {
            if (_throwException)
                throw new Exception("Repository exception");

            return Task.FromResult(_response);
        }
    }

    [TestFixture]
    public class GetStatisticOverviewHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnResponse_WhenDataExists()
        {
            // Arrange
            var request = new GetStatisticOverviewRequest { Month = 8, Year = 2025 };

            var expectedResponse = new GetStatisticOverviewResponse
            {
                TotalRevenue = 10000m,
                SuccessfulAuctions = 3,
                TotalParticipants = 50,
            };

            var fakeRepo = new FakeGetStatisticOverviewRepository(expectedResponse);
            var handler = new GetStatisticOverviewHandler(fakeRepo);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.TotalRevenue, result.TotalRevenue);
            Assert.AreEqual(expectedResponse.SuccessfulAuctions, result.SuccessfulAuctions);
            Assert.AreEqual(expectedResponse.TotalParticipants, result.TotalParticipants);
        }

        [Test]
        public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var request = new GetStatisticOverviewRequest { Month = 7, Year = 2025 };

            var fakeRepo = new FakeGetStatisticOverviewRepository(null);
            var handler = new GetStatisticOverviewHandler(fakeRepo);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Handle_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            var request = new GetStatisticOverviewRequest { Month = 6, Year = 2025 };

            var fakeRepo = new FakeGetStatisticOverviewRepository(null, throwException: true);
            var handler = new GetStatisticOverviewHandler(fakeRepo);

            // Act + Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await handler.Handle(request, CancellationToken.None)
            );
        }
    }
}
