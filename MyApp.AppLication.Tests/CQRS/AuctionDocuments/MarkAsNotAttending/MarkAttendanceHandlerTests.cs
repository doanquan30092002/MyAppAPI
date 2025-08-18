using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.AuctionDocuments.MarkAsNotAttending;
using MyApp.Application.Interfaces.IAuctionDocuments;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.MarkAsNotAttending.Tests
{
    [TestFixture]
    public class MarkAttendanceHandlerTests
    {
        private Mock<IAuctionDocuments> _mockRepo;
        private MarkAttendanceHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IAuctionDocuments>();
            _handler = new MarkAttendanceHandler(_mockRepo.Object);
        }

        [Test]
        public void Handle_DocumentNotFound_ThrowsKeyNotFoundException()
        {
            var docId = Guid.NewGuid();
            var request = new MarkAttendanceRequest
            {
                AuctionDocumentIds = new List<Guid> { docId },
                IsAttended = true,
            };

            _mockRepo
                .Setup(r => r.GetDocumentByIdAsync(docId))
                .ReturnsAsync((MyApp.Core.Entities.AuctionDocuments)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );

            Assert.IsTrue(ex.Message.Contains(docId.ToString()));
        }

        [Test]
        public void Handle_AuctionNotFound_ThrowsInvalidOperationException()
        {
            var docId = Guid.NewGuid();
            var request = new MarkAttendanceRequest
            {
                AuctionDocumentIds = new List<Guid> { docId },
                IsAttended = true,
            };

            _mockRepo
                .Setup(r => r.GetDocumentByIdAsync(docId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.AuctionDocuments { AuctionDocumentsId = docId }
                );
            _mockRepo
                .Setup(r => r.GetAuctionByAuctionDocumentIdAsync(docId))
                .ReturnsAsync((MyApp.Core.Entities.Auction)null);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );

            Assert.IsTrue(ex.Message.Contains(docId.ToString()));
        }

        [Test]
        public void Handle_AuctionNotStarted_ThrowsInvalidOperationException()
        {
            var docId = Guid.NewGuid();
            var request = new MarkAttendanceRequest
            {
                AuctionDocumentIds = new List<Guid> { docId },
                IsAttended = true,
            };

            _mockRepo
                .Setup(r => r.GetDocumentByIdAsync(docId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.AuctionDocuments { AuctionDocumentsId = docId }
                );
            _mockRepo
                .Setup(r => r.GetAuctionByAuctionDocumentIdAsync(docId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionStartDate = DateTime.Now.AddHours(1), // future
                    }
                );

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );

            Assert.IsTrue(ex.Message.Contains(docId.ToString()));
        }

        [Test]
        public async Task Handle_Success_ReturnsTrue()
        {
            var docId = Guid.NewGuid();
            var request = new MarkAttendanceRequest
            {
                AuctionDocumentIds = new List<Guid> { docId },
                IsAttended = true,
            };

            _mockRepo
                .Setup(r => r.GetDocumentByIdAsync(docId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.AuctionDocuments { AuctionDocumentsId = docId }
                );
            _mockRepo
                .Setup(r => r.GetAuctionByAuctionDocumentIdAsync(docId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Auction
                    {
                        AuctionStartDate = DateTime.Now.AddHours(-1), // past
                    }
                );
            _mockRepo
                .Setup(r => r.UpdateIsAttendedAsync(request.AuctionDocumentIds, request.IsAttended))
                .ReturnsAsync(true);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsTrue(result);
        }
    }
}
