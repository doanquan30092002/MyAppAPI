using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.CQRS.AuctionDocuments.ConfirmReufund;
using MyApp.Application.Interfaces.INotificationsRepository;
using MyApp.Application.Interfaces.IRefundRepository;
using MyApp.Application.Interfaces.IUnitOfWork;
using NUnit.Framework;

namespace MyApp.Application.CQRS.AuctionDocuments.ConfirmReufund.Tests
{
    [TestFixture]
    public class ConfirmRefundHandlerTests
    {
        private Mock<IRefundRepository> _refundRepoMock;
        private Mock<INotificationsRepository> _notificationRepoMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private ConfirmRefundHandler _handler;

        [SetUp]
        public void Setup()
        {
            _refundRepoMock = new Mock<IRefundRepository>();
            _notificationRepoMock = new Mock<INotificationsRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new ConfirmRefundHandler(
                _refundRepoMock.Object,
                _notificationRepoMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Test]
        public async Task Handle_AuctionDocumentIdsIsNull_ReturnsFalse()
        {
            var request = new ConfirmRefundCommand { AuctionDocumentIds = null };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsFalse(result);
            _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Never);
        }

        [Test]
        public async Task Handle_AuctionDocumentIdsIsEmpty_ReturnsFalse()
        {
            var request = new ConfirmRefundCommand { AuctionDocumentIds = new List<Guid>() };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsFalse(result);
            _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Never);
        }

        [Test]
        public async Task Handle_ConfirmRefundAsyncReturnsFalse_RollsBackAndReturnsFalse()
        {
            var request = new ConfirmRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
            };

            _refundRepoMock.Setup(r => r.ConfirmRefundAsync(request)).ReturnsAsync(false);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsFalse(result);
            _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_ConfirmRefundAsyncReturnsTrue_NoDocuments_CommitsAndReturnsTrue()
        {
            var request = new ConfirmRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { Guid.NewGuid() },
            };

            _refundRepoMock.Setup(r => r.ConfirmRefundAsync(request)).ReturnsAsync(true);
            _refundRepoMock
                .Setup(r => r.GetAuctionDocumentsByIdsAsync(request.AuctionDocumentIds))
                .ReturnsAsync(new List<MyApp.Core.Entities.AuctionDocuments>());

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsTrue(result);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _notificationRepoMock.Verify(
                n => n.SaveNotificationsAsync(It.IsAny<List<MyApp.Core.Entities.Notification>>()),
                Times.Never
            );
        }

        [Test]
        public async Task Handle_ConfirmRefundAsyncReturnsTrue_WithDocuments_SavesNotificationsAndCommits()
        {
            var doc = new MyApp.Core.Entities.AuctionDocuments
            {
                AuctionDocumentsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };
            var request = new ConfirmRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { doc.AuctionDocumentsId },
            };

            _refundRepoMock.Setup(r => r.ConfirmRefundAsync(request)).ReturnsAsync(true);
            _refundRepoMock
                .Setup(r => r.GetAuctionDocumentsByIdsAsync(request.AuctionDocumentIds))
                .ReturnsAsync(new List<MyApp.Core.Entities.AuctionDocuments> { doc });
            _notificationRepoMock
                .Setup(n =>
                    n.SaveNotificationsAsync(It.IsAny<List<MyApp.Core.Entities.Notification>>())
                )
                .ReturnsAsync(true);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsTrue(result);
            _notificationRepoMock.Verify(
                n =>
                    n.SaveNotificationsAsync(
                        It.Is<List<MyApp.Core.Entities.Notification>>(l => l.Count == 1)
                    ),
                Times.Once
            );
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_SaveNotificationsThrowsException_RollsBackAndReturnsFalse()
        {
            var doc = new MyApp.Core.Entities.AuctionDocuments
            {
                AuctionDocumentsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };
            var request = new ConfirmRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { doc.AuctionDocumentsId },
            };

            _refundRepoMock.Setup(r => r.ConfirmRefundAsync(request)).ReturnsAsync(true);
            _refundRepoMock
                .Setup(r => r.GetAuctionDocumentsByIdsAsync(request.AuctionDocumentIds))
                .ReturnsAsync(new List<MyApp.Core.Entities.AuctionDocuments> { doc });
            _notificationRepoMock
                .Setup(n =>
                    n.SaveNotificationsAsync(It.IsAny<List<MyApp.Core.Entities.Notification>>())
                )
                .ThrowsAsync(new Exception("DB error"));

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsFalse(result);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}
