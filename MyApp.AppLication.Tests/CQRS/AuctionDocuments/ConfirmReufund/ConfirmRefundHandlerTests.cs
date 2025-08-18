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
        public async Task Handle_ShouldReturnFalse_WhenAuctionDocumentIdsNullOrEmpty()
        {
            // Arrange
            var command1 = new ConfirmRefundCommand { AuctionDocumentIds = null };
            var command2 = new ConfirmRefundCommand { AuctionDocumentIds = new List<Guid>() };

            // Act
            var result1 = await _handler.Handle(command1, CancellationToken.None);
            var result2 = await _handler.Handle(command2, CancellationToken.None);

            // Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenConfirmRefundFails()
        {
            // Arrange
            var docId = Guid.NewGuid();
            var command = new ConfirmRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { docId },
            };

            _refundRepoMock.Setup(x => x.ConfirmRefundAsync(command)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.BeginTransaction());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
            _unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenConfirmRefundSucceeds()
        {
            // Arrange
            var docId = Guid.NewGuid();
            var command = new ConfirmRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { docId },
            };

            var documents = new List<MyApp.Core.Entities.AuctionDocuments>
            {
                new MyApp.Core.Entities.AuctionDocuments
                {
                    AuctionDocumentsId = docId,
                    UserId = Guid.NewGuid(),
                },
            };

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _refundRepoMock.Setup(x => x.ConfirmRefundAsync(command)).ReturnsAsync(true);
            _refundRepoMock
                .Setup(x => x.GetAuctionDocumentsByIdsAsync(command.AuctionDocumentIds))
                .ReturnsAsync(documents);
            _notificationRepoMock
                .Setup(x =>
                    x.SaveNotificationsAsync(It.IsAny<List<MyApp.Core.Entities.Notification>>())
                )
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
            _notificationRepoMock.Verify(
                x => x.SaveNotificationsAsync(It.IsAny<List<MyApp.Core.Entities.Notification>>()),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenExceptionOccurs()
        {
            // Arrange
            var docId = Guid.NewGuid();
            var command = new ConfirmRefundCommand
            {
                AuctionDocumentIds = new List<Guid> { docId },
            };

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _refundRepoMock.Setup(x => x.ConfirmRefundAsync(command)).ThrowsAsync(new Exception());
            _unitOfWorkMock.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
            _unitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Once);
        }
    }
}
