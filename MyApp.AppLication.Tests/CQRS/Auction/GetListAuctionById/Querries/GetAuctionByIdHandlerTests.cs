using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.Auction.GetListAuctionById.Querries;
using MyApp.Application.Interfaces.IGetAuctionByIdRepository;
using NUnit.Framework;

namespace MyApp.Tests.CQRS.Auction
{
    [TestFixture]
    public class GetAuctionByIdHandlerTests
    {
        private Mock<IGetAuctionByIdRepository> _repoMock;
        private GetAuctionByIdHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IGetAuctionByIdRepository>();
            _handler = new GetAuctionByIdHandler(_repoMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnAuction_WhenAuctionExists()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var request = new GetAuctionByIdRequest { AuctionId = auctionId };

            var expectedResponse = new GetAuctionByIdResponse
            {
                AuctionName = "Test Auction",
                AuctionDescription = "Some description",
            };

            _repoMock.Setup(r => r.GetAuctionByIdAsync(auctionId)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Auction", result.AuctionName);
            _repoMock.Verify(r => r.GetAuctionByIdAsync(auctionId), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var request = new GetAuctionByIdRequest { AuctionId = auctionId };

            _repoMock
                .Setup(r => r.GetAuctionByIdAsync(auctionId))
                .ReturnsAsync((GetAuctionByIdResponse)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }
    }
}
