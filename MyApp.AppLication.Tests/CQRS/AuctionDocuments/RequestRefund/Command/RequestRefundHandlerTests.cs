using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Command;
using MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Helper;
using MyApp.Application.Interfaces.IAuctionDocuments;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Command.Tests
{
    [TestFixture]
    public class RequestRefundHandlerTests
    {
        private Mock<IAuctionDocuments> _auctionDocumentsRepoMock;
        private Mock<IRequestRefundHelper> _helperMock;
        private RequestRefundHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionDocumentsRepoMock = new Mock<IAuctionDocuments>();
            _helperMock = new Mock<IRequestRefundHelper>();
            _handler = new RequestRefundHandler(
                _auctionDocumentsRepoMock.Object,
                _helperMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenRequestIsValid()
        {
            // Arrange
            var request = new RequestRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                RefundProof = Mock.Of<IFormFile>(),
                RefundReason = "Lý do test",
            };

            var userId = Guid.NewGuid();

            _helperMock.Setup(h => h.GetCurrentUserId()).Returns(userId);
            _helperMock
                .Setup(h => h.ValidateDocumentForRefundAsync(It.IsAny<Guid>(), userId))
                .Returns(Task.CompletedTask);
            _helperMock
                .Setup(h => h.UploadRefundProofAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("http://file-url.com");

            _auctionDocumentsRepoMock
                .Setup(r =>
                    r.RequestRefundAsync(
                        request.AuctionDocumentIds,
                        userId,
                        "http://file-url.com",
                        request.RefundReason
                    )
                )
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Handle_ShouldThrowUnauthorized_WhenUserIdInvalid()
        {
            // Arrange
            var request = new RequestRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                RefundProof = Mock.Of<IFormFile>(),
                RefundReason = "Test",
            };

            _helperMock
                .Setup(h => h.GetCurrentUserId())
                .Throws(new UnauthorizedAccessException("Không thể xác định người dùng hiện tại."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );
            StringAssert.Contains("Không thể xác định người dùng hiện tại.", ex.Message);
        }

        [Test]
        public void Handle_ShouldThrow_WhenValidateDocumentFails()
        {
            // Arrange
            var request = new RequestRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                RefundProof = Mock.Of<IFormFile>(),
                RefundReason = "Test",
            };

            var userId = Guid.NewGuid();

            _helperMock.Setup(h => h.GetCurrentUserId()).Returns(userId);
            _helperMock
                .Setup(h => h.ValidateDocumentForRefundAsync(It.IsAny<Guid>(), userId))
                .ThrowsAsync(new InvalidOperationException("Lỗi hồ sơ."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );
            StringAssert.Contains("Lỗi hồ sơ.", ex.Message);
        }

        [Test]
        public void Handle_ShouldThrow_WhenUploadProofFails()
        {
            // Arrange
            var request = new RequestRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                RefundProof = Mock.Of<IFormFile>(),
                RefundReason = "Test",
            };

            var userId = Guid.NewGuid();

            _helperMock.Setup(h => h.GetCurrentUserId()).Returns(userId);
            _helperMock
                .Setup(h => h.ValidateDocumentForRefundAsync(It.IsAny<Guid>(), userId))
                .Returns(Task.CompletedTask);
            _helperMock
                .Setup(h => h.UploadRefundProofAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new InvalidOperationException("Upload file thất bại"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );
            StringAssert.Contains("Upload file thất bại", ex.Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var request = new RequestRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                RefundProof = Mock.Of<IFormFile>(),
                RefundReason = "Test",
            };

            var userId = Guid.NewGuid();

            _helperMock.Setup(h => h.GetCurrentUserId()).Returns(userId);
            _helperMock
                .Setup(h => h.ValidateDocumentForRefundAsync(It.IsAny<Guid>(), userId))
                .Returns(Task.CompletedTask);
            _helperMock
                .Setup(h => h.UploadRefundProofAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("http://file-url.com");

            _auctionDocumentsRepoMock
                .Setup(r =>
                    r.RequestRefundAsync(
                        request.AuctionDocumentIds,
                        userId,
                        "http://file-url.com",
                        request.RefundReason
                    )
                )
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
