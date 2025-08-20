using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Common.Services.SendMessage;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.INotificationsRepository;
using MyApp.Application.Interfaces.IUnitOfWork;

namespace MyApp.Application.CQRS.Auction.CancelAuction.Commands.Tests
{
    [TestFixture]
    public class CancelAuctionHandlerTests
    {
        private Mock<IAuctionRepository> _auctionRepoMock;
        private Mock<ICurrentUserService> _currentUserMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ISendMessage> _emailSenderMock;
        private Mock<INotificationsRepository> _notificationRepoMock;
        private Mock<INotificationSender> _notificationSenderMock;
        private CancelAuctionHandler _handler;

        private Guid _auctionId;
        private CancelAuctionCommand _command;
        private string _userIdStr;
        private Guid _userIdGuid;

        [SetUp]
        public void Setup()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailSenderMock = new Mock<ISendMessage>();
            _notificationRepoMock = new Mock<INotificationsRepository>();
            _notificationSenderMock = new Mock<INotificationSender>();

            _auctionId = Guid.NewGuid();
            _command = new CancelAuctionCommand
            {
                AuctionId = _auctionId,
                CancelReason = "Test reason",
            };
            _userIdStr = Guid.NewGuid().ToString();
            _userIdGuid = Guid.Parse(_userIdStr);

            _handler = new CancelAuctionHandler(
                _auctionRepoMock.Object,
                _currentUserMock.Object,
                _unitOfWorkMock.Object,
                new List<ISendMessage> { _emailSenderMock.Object },
                _notificationRepoMock.Object,
                _notificationSenderMock.Object
            );
        }

        [Test]
        public void Handle_ShouldThrowUnauthorized_WhenUserIdNullOrEmpty()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns((string?)null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(_command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_ShouldThrowUnauthorized_WhenUserIdInvalidGuid()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns("invalid-guid");

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(_command, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ShouldCancelAuctionSuccessfully_AndSendNotificationsAndEmails()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns(_userIdStr);

            var documents = new List<MyApp.Core.Entities.AuctionDocuments>
            {
                new() { UserId = Guid.NewGuid() },
                new() { UserId = Guid.NewGuid() },
            };
            _auctionRepoMock
                .Setup(x => x.CancelAuctionAsync(_command, _userIdGuid))
                .ReturnsAsync(true);
            _auctionRepoMock
                .Setup(x => x.GetPaidOrDepositedDocumentsByAuctionIdAsync(_auctionId))
                .ReturnsAsync(documents);
            _auctionRepoMock
                .Setup(x => x.GetEmailsByUserIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<string> { "test@example.com" });

            _emailSenderMock.Setup(x => x.Channel).Returns(SendMessageChannel.Email);
            _emailSenderMock
                .Setup(x =>
                    x.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<List<string>>()
                    )
                )
                .ReturnsAsync(true);

            _notificationRepoMock
                .Setup(x =>
                    x.SaveNotificationsAsync(It.IsAny<List<MyApp.Core.Entities.Notification>>())
                )
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.BeginTransaction());

            var result = await _handler.Handle(_command, CancellationToken.None);

            Assert.IsTrue(result);
            _unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Once);
            _auctionRepoMock.Verify(x => x.CancelAuctionAsync(_command, _userIdGuid), Times.Once);
            _notificationRepoMock.Verify(
                x => x.SaveNotificationsAsync(It.IsAny<List<MyApp.Core.Entities.Notification>>()),
                Times.Once
            );
            _emailSenderMock.Verify(
                x =>
                    x.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<List<string>>()
                    ),
                Times.Once
            );
            _notificationSenderMock.Verify(
                x => x.SendToUsersAsync(It.IsAny<List<Guid>>(), It.IsAny<object>()),
                Times.Once
            );
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public void Handle_ShouldRollback_WhenExceptionThrown()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns(_userIdStr);
            _auctionRepoMock
                .Setup(x => x.CancelAuctionAsync(_command, _userIdGuid))
                .ThrowsAsync(new Exception("DB error"));

            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_command, CancellationToken.None));
            _unitOfWorkMock.Verify(x => x.RollbackAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldHandleNoEmailsOrNotificationsGracefully()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns(_userIdStr);
            _auctionRepoMock
                .Setup(x => x.CancelAuctionAsync(_command, _userIdGuid))
                .ReturnsAsync(true);
            _auctionRepoMock
                .Setup(x => x.GetPaidOrDepositedDocumentsByAuctionIdAsync(_auctionId))
                .ReturnsAsync(new List<MyApp.Core.Entities.AuctionDocuments>());
            _auctionRepoMock
                .Setup(x => x.GetEmailsByUserIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<string>()); // email list rỗng

            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.BeginTransaction());

            var result = await _handler.Handle(_command, CancellationToken.None);

            Assert.IsTrue(result);

            _notificationRepoMock.Verify(
                x => x.SaveNotificationsAsync(It.IsAny<List<MyApp.Core.Entities.Notification>>()),
                Times.Never
            );
            _emailSenderMock.Verify(
                x =>
                    x.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<List<string>>()
                    ),
                Times.Never
            );
            _notificationSenderMock.Verify(
                x => x.SendToUsersAsync(It.IsAny<List<Guid>>(), It.IsAny<object>()),
                Times.Never
            );
        }

        [Test]
        public async Task Handle_ShouldSkipEmail_WhenEmailSenderNull()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns(_userIdStr);

            _auctionRepoMock
                .Setup(x => x.CancelAuctionAsync(_command, _userIdGuid))
                .ReturnsAsync(true);

            _auctionRepoMock
                .Setup(x => x.GetPaidOrDepositedDocumentsByAuctionIdAsync(_auctionId))
                .ReturnsAsync(new List<MyApp.Core.Entities.AuctionDocuments>());

            // Không có email sender trong danh sách
            var handler = new CancelAuctionHandler(
                _auctionRepoMock.Object,
                _currentUserMock.Object,
                _unitOfWorkMock.Object,
                new List<ISendMessage>(), // trống
                _notificationRepoMock.Object,
                _notificationSenderMock.Object
            );

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var result = await handler.Handle(_command, CancellationToken.None);

            Assert.IsTrue(result);
            _emailSenderMock.Verify(
                x =>
                    x.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<List<string>>()
                    ),
                Times.Never
            );
        }

        [Test]
        public async Task Handle_ShouldSkipEmail_WhenEmailListEmpty()
        {
            _currentUserMock.Setup(x => x.GetUserId()).Returns(_userIdStr);

            _auctionRepoMock
                .Setup(x => x.CancelAuctionAsync(_command, _userIdGuid))
                .ReturnsAsync(true);

            _auctionRepoMock
                .Setup(x => x.GetPaidOrDepositedDocumentsByAuctionIdAsync(_auctionId))
                .ReturnsAsync(new List<MyApp.Core.Entities.AuctionDocuments>());

            _auctionRepoMock
                .Setup(x => x.GetEmailsByUserIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<string>()); // email list rỗng

            _unitOfWorkMock.Setup(x => x.BeginTransaction());
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(_command, CancellationToken.None);

            Assert.IsTrue(result);
            _emailSenderMock.Verify(
                x =>
                    x.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<List<string>>()
                    ),
                Times.Never
            );
        }
    }
}
