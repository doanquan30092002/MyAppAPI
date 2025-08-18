using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.Auction.MarkAuctionAsSuccessful.Command;
using MyApp.Application.Interfaces.IAuctionRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Auction.MarkAuctionAsSuccessful.Command.Tests
{
    [TestFixture]
    public class MarkAuctionAsSuccessfulHandlerTests
    {
        private Mock<IAuctionRepository> _auctionRepositoryMock;
        private MarkAuctionAsSuccessfulHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _handler = new MarkAuctionAsSuccessfulHandler(_auctionRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var command = new MarkAuctionAsSuccessfulCommand { AuctionId = auctionId };

            _auctionRepositoryMock
                .Setup(r => r.MarkAuctionAsSuccessfulAsync(auctionId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            _auctionRepositoryMock.Verify(
                r => r.MarkAuctionAsSuccessfulAsync(auctionId),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var command = new MarkAuctionAsSuccessfulCommand { AuctionId = auctionId };

            _auctionRepositoryMock
                .Setup(r => r.MarkAuctionAsSuccessfulAsync(auctionId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
            _auctionRepositoryMock.Verify(
                r => r.MarkAuctionAsSuccessfulAsync(auctionId),
                Times.Once
            );
        }
    }
}
