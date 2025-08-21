using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.AuctionDocuments.ReviewRequestRefund.Command;
using MyApp.Application.Interfaces.IAuctionDocuments;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.ReviewRequestRefund.Command.Tests
{
    [TestFixture]
    public class ReviewRequestRefundHandlerTests
    {
        private Mock<IAuctionDocuments> _auctionDocumentsRepo;
        private ReviewRequestRefundHandler _handler;

        [SetUp]
        public void Setup()
        {
            _auctionDocumentsRepo = new Mock<IAuctionDocuments>();
            _handler = new ReviewRequestRefundHandler(_auctionDocumentsRepo.Object);
        }

        [Test]
        public void Handle_ShouldThrow_WhenDocumentNotFound()
        {
            // Arrange
            var request = new ReviewRequestRefundRequest
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                StatusRefund = 2,
                NoteReviewRefund = "note",
            };

            _auctionDocumentsRepo
                .Setup(r => r.GetDocumentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((MyApp.Core.Entities.AuctionDocuments)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrow_WhenStatusRefundIs2AndDepositNot1()
        {
            var doc = new MyApp.Core.Entities.AuctionDocuments
            {
                StatusDeposit = 0,
                StatusRefund = 1,
            };

            var request = new ReviewRequestRefundRequest
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                StatusRefund = 2,
            };

            _auctionDocumentsRepo
                .Setup(r => r.GetDocumentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(doc);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrow_WhenStatusRefundNot1_Null()
        {
            var doc = new MyApp.Core.Entities.AuctionDocuments
            {
                StatusDeposit = 1,
                StatusRefund = null,
            };

            var request = new ReviewRequestRefundRequest
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                StatusRefund = 2,
            };

            _auctionDocumentsRepo
                .Setup(r => r.GetDocumentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(doc);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrow_WhenStatusRefundNot1_2()
        {
            var doc = new MyApp.Core.Entities.AuctionDocuments
            {
                StatusDeposit = 1,
                StatusRefund = 2,
            };

            var request = new ReviewRequestRefundRequest
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                StatusRefund = 2,
            };

            _auctionDocumentsRepo
                .Setup(r => r.GetDocumentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(doc);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrow_WhenStatusRefundNot1_3()
        {
            var doc = new MyApp.Core.Entities.AuctionDocuments
            {
                StatusDeposit = 1,
                StatusRefund = 3,
            };

            var request = new ReviewRequestRefundRequest
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                StatusRefund = 2,
            };

            _auctionDocumentsRepo
                .Setup(r => r.GetDocumentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(doc);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenValid()
        {
            var doc = new MyApp.Core.Entities.AuctionDocuments
            {
                StatusDeposit = 1,
                StatusRefund = 1,
            };

            var request = new ReviewRequestRefundRequest
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                StatusRefund = 2,
                NoteReviewRefund = "ok",
            };

            _auctionDocumentsRepo
                .Setup(r => r.GetDocumentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(doc);

            _auctionDocumentsRepo
                .Setup(r =>
                    r.ReviewRequestRefundAsync(
                        It.IsAny<List<Guid>>(),
                        It.IsAny<int>(),
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync(true);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenRepoReturnsFalse()
        {
            var doc = new MyApp.Core.Entities.AuctionDocuments
            {
                StatusDeposit = 1,
                StatusRefund = 1,
            };

            var request = new ReviewRequestRefundRequest
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
                StatusRefund = 2,
            };

            _auctionDocumentsRepo
                .Setup(r => r.GetDocumentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(doc);

            _auctionDocumentsRepo
                .Setup(r =>
                    r.ReviewRequestRefundAsync(
                        It.IsAny<List<Guid>>(),
                        It.IsAny<int>(),
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync(false);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsFalse(result);
        }
    }
}
