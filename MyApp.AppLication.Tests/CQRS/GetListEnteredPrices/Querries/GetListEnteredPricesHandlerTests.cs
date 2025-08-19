using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetListEnteredPrices.Querries;
using MyApp.Application.Interfaces.IGetListEnteredPricesRepository;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.Application.CQRS.GetListEnteredPrices.Queries.Tests
{
    // Fake repository để test
    public class FakeGetListEnteredPricesRepository : IGetListEnteredPricesRepository
    {
        private readonly List<AuctionRoundPrices>? _data;

        public FakeGetListEnteredPricesRepository(List<AuctionRoundPrices>? data)
        {
            _data = data;
        }

        public Task<List<AuctionRoundPrices>?> GetListEnteredPricesAsync(Guid auctionRoundId)
        {
            // Lọc dữ liệu theo auctionRoundId nếu có
            if (_data == null)
                return Task.FromResult<List<AuctionRoundPrices>?>(null);

            return Task.FromResult(_data.Where(x => x.AuctionRoundId == auctionRoundId).ToList());
        }
    }

    [TestFixture]
    public class GetListEnteredPricesHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnEmptyList_WhenNoDataFound()
        {
            // Arrange
            var auctionRoundId = Guid.NewGuid();
            var fakeRepo = new FakeGetListEnteredPricesRepository(new List<AuctionRoundPrices>()); // rỗng
            var handler = new GetListEnteredPricesHandler(fakeRepo);
            var request = new GetListEnteredPricesRequest { AuctionRoundId = auctionRoundId };

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ListAuctionRoundPrices);
            Assert.AreEqual(0, response.ListAuctionRoundPrices.Count);
        }

        [Test]
        public async Task Handle_ShouldReturnList_WhenDataExists()
        {
            // Arrange
            var auctionRoundId = Guid.NewGuid();
            var expected = new List<AuctionRoundPrices>
            {
                new AuctionRoundPrices
                {
                    AuctionRoundPriceId = Guid.NewGuid(),
                    AuctionRoundId = auctionRoundId,
                    UserName = "User A",
                    CitizenIdentification = "123456",
                    RecentLocation = "HN",
                    TagName = "Tag1",
                    AuctionPrice = 1000,
                    CreatedBy = Guid.NewGuid(),
                    FlagWinner = false,
                },
                new AuctionRoundPrices
                {
                    AuctionRoundPriceId = Guid.NewGuid(),
                    AuctionRoundId = auctionRoundId,
                    UserName = "User B",
                    CitizenIdentification = "654321",
                    RecentLocation = "HCM",
                    TagName = "Tag2",
                    AuctionPrice = 2000,
                    CreatedBy = Guid.NewGuid(),
                    FlagWinner = true,
                },
            };

            var fakeRepo = new FakeGetListEnteredPricesRepository(expected);
            var handler = new GetListEnteredPricesHandler(fakeRepo);
            var request = new GetListEnteredPricesRequest { AuctionRoundId = auctionRoundId };

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.ListAuctionRoundPrices.Count);
            Assert.AreEqual("User A", response.ListAuctionRoundPrices[0].UserName);
            Assert.AreEqual("User B", response.ListAuctionRoundPrices[1].UserName);
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            var auctionRoundId = Guid.NewGuid();
            var fakeRepo = new FakeGetListEnteredPricesRepository(null); // repo trả về null
            var handler = new GetListEnteredPricesHandler(fakeRepo);
            var request = new GetListEnteredPricesRequest { AuctionRoundId = auctionRoundId };

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ListAuctionRoundPrices);
            Assert.AreEqual(0, response.ListAuctionRoundPrices.Count);
        }
    }
}
