using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetListUserWinner.Queries;
using MyApp.Application.CQRS.GetListUserWinner.Querries;
using MyApp.Application.Interfaces.IListUserWinnerRepository;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetListUserWinner.Queries.Tests
{
    public class FakeListUserWinnerRepository : IListUserWinnerRepository
    {
        private readonly Func<GetListUserWinnerRequest, Task<GetListUserWinnerResponse>> _handler;

        public FakeListUserWinnerRepository(
            Func<GetListUserWinnerRequest, Task<GetListUserWinnerResponse>> handler
        )
        {
            _handler = handler;
        }

        public Task<GetListUserWinnerResponse> GetListUserWinnerAsync(
            GetListUserWinnerRequest request
        )
        {
            return _handler(request);
        }
    }

    [TestFixture]
    public class GetListUserWinnerHandlerTests
    {
        [Test]
        public void Handle_ShouldThrowArgumentException_WhenAuctionIdIsEmpty()
        {
            // Arrange
            var repo = new FakeListUserWinnerRepository(request =>
            {
                if (request == null || request.AuctionId == Guid.Empty)
                    throw new ArgumentException("ID phiên không hợp lệ");
                return Task.FromResult(new GetListUserWinnerResponse());
            });

            var handler = new GetListUserWinnerHandler(repo);
            var request = new GetListUserWinnerRequest { AuctionId = Guid.Empty };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyResponse_WhenAuctionNotCompleted()
        {
            // Arrange
            var repo = new FakeListUserWinnerRepository(request =>
            {
                return Task.FromResult(
                    new GetListUserWinnerResponse
                    {
                        auctionRoundPrices = new List<AuctionRoundPrices>(),
                    }
                );
            });

            var handler = new GetListUserWinnerHandler(repo);
            var request = new GetListUserWinnerRequest { AuctionId = Guid.NewGuid() };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsEmpty(result.auctionRoundPrices);
        }

        [Test]
        public async Task Handle_ShouldReturnWinners_WhenValidAuction()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var winner = new AuctionRoundPrices
            {
                AuctionRoundPriceId = Guid.NewGuid(),
                UserName = "testuser",
                TagName = "asset-001",
                AuctionPrice = 1000,
                FlagWinner = true,
                AuctionRound = new AuctionRound
                {
                    AuctionId = auctionId,
                    RoundNumber = 1,
                    Status = 2,
                },
            };

            var repo = new FakeListUserWinnerRepository(request =>
            {
                return Task.FromResult(
                    new GetListUserWinnerResponse
                    {
                        auctionRoundPrices = new List<AuctionRoundPrices> { winner },
                    }
                );
            });

            var handler = new GetListUserWinnerHandler(repo);
            var request = new GetListUserWinnerRequest { AuctionId = auctionId };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.auctionRoundPrices.Count);
            Assert.AreEqual("testuser", result.auctionRoundPrices[0].UserName);
            Assert.AreEqual(1000, result.auctionRoundPrices[0].AuctionPrice);
            Assert.IsTrue(result.auctionRoundPrices[0].FlagWinner);
        }
    }
}
