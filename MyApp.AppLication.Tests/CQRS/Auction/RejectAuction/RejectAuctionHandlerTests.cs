using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.Auction.RejectAuction;
using MyApp.Application.Interfaces.IAuctionRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Auction.RejectAuction.Tests
{
    [TestFixture]
    public class RejectAuctionHandlerTests
    {
        private Mock<IAuctionRepository> _auctionRepoMock;
        private RejectAuctionHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _handler = new RejectAuctionHandler(_auctionRepoMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var request = new RejectAuction
            {
                AuctionId = Guid.NewGuid(),
                RejectReason = "Lý do test",
            };
            _auctionRepoMock
                .Setup(x => x.RejectAuctionAsync(request.AuctionId, request.RejectReason))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            _auctionRepoMock.Verify(
                x => x.RejectAuctionAsync(request.AuctionId, request.RejectReason),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var request = new RejectAuction
            {
                AuctionId = Guid.NewGuid(),
                RejectReason = "Không hợp lệ",
            };
            _auctionRepoMock
                .Setup(x => x.RejectAuctionAsync(request.AuctionId, request.RejectReason))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
            _auctionRepoMock.Verify(
                x => x.RejectAuctionAsync(request.AuctionId, request.RejectReason),
                Times.Once
            );
        }

        [Test]
        public void RejectAuction_ShouldFailValidation_WhenAuctionIdMissing()
        {
            // Arrange
            var request = new RejectAuction
            {
                RejectReason = "Lý do test",
                // AuctionId mặc định = Guid.Empty
            };

            var context = new ValidationContext(request);
            var results = new System.Collections.Generic.List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.That(results[0].ErrorMessage, Is.EqualTo("Phiên đấu giá là bắt buộc."));
        }

        [Test]
        public void RejectAuction_ShouldFailValidation_WhenRejectReasonMissing()
        {
            // Arrange
            var request = new RejectAuction
            {
                AuctionId = Guid.NewGuid(),
                RejectReason = "", // thiếu lý do
            };

            var context = new ValidationContext(request);
            var results = new System.Collections.Generic.List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.That(results[0].ErrorMessage, Is.EqualTo("Lý do từ chối là bắt buộc."));
        }

        [Test]
        public void RejectAuction_ShouldPassValidation_WhenAllFieldsValid()
        {
            // Arrange
            var request = new RejectAuction { AuctionId = Guid.NewGuid(), RejectReason = "OK" };

            var context = new ValidationContext(request);
            var results = new System.Collections.Generic.List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            Assert.IsTrue(isValid);
            Assert.That(results, Is.Empty);
        }
    }
}
