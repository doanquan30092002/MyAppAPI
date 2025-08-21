using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Interfaces.ReceiveAuctionRegistrationForm;

namespace MyApp.Application.CQRS.ReceiveAuctionRegistrationForm.Tests
{
    [TestFixture()]
    public class ReceiveAuctionRegistrationFormHandlerTests
    {
        private Mock<IReceiveAuctionRegistrationFormRepository> _repositoryMock;
        private Mock<INotificationSender> _notificationSenderMock;
        private ReceiveAuctionRegistrationFormHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IReceiveAuctionRegistrationFormRepository>();
            _notificationSenderMock = new Mock<INotificationSender>();

            _handler = new ReceiveAuctionRegistrationFormHandler(
                _repositoryMock.Object,
                _notificationSenderMock.Object
            );
        }

        [Test]
        public async Task Handle_WhenUpdateStatusReturnsTrue_ShouldSendAndSaveNotification()
        {
            // Arrange
            var request = new ReceiveAuctionRegistrationFormRequest
            {
                AuctionDocumentsId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            };

            var userIds = new List<Guid>
            {
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            };
            var auctionName = "Phiên đấu giá xe ô tô";

            _repositoryMock
                .Setup(r => r.UpdateStatusTicketSigned(request.AuctionDocumentsId))
                .ReturnsAsync(true);

            _repositoryMock
                .Setup(r => r.GetUserIdByAuctionDocumentId(request.AuctionDocumentsId))
                .ReturnsAsync(userIds);

            _repositoryMock
                .Setup(r => r.GetAuctionNameByAuctionDocumentsIdAsync(request.AuctionDocumentsId))
                .ReturnsAsync(auctionName);

            _repositoryMock
                .Setup(r => r.SaveNotificationAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _notificationSenderMock
                .Setup(s => s.SendToUsersAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var expectedMessage = string.Format(Message.RECEIVED_FORM_SUCCESS, auctionName);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            _repositoryMock.Verify(
                r => r.UpdateStatusTicketSigned(request.AuctionDocumentsId),
                Times.Once
            );
            _repositoryMock.Verify(
                r => r.GetUserIdByAuctionDocumentId(request.AuctionDocumentsId),
                Times.Once
            );
            _repositoryMock.Verify(
                r => r.GetAuctionNameByAuctionDocumentsIdAsync(request.AuctionDocumentsId),
                Times.Once
            );
            _notificationSenderMock.Verify(
                s => s.SendToUsersAsync(userIds, expectedMessage),
                Times.Once
            );
            _repositoryMock.Verify(
                r => r.SaveNotificationAsync(userIds, expectedMessage),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_WhenUpdateStatusReturnsFalse_ShouldNotSendNotification()
        {
            // Arrange
            var request = new ReceiveAuctionRegistrationFormRequest
            {
                AuctionDocumentsId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            };

            _repositoryMock
                .Setup(r => r.UpdateStatusTicketSigned(request.AuctionDocumentsId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
            _repositoryMock.Verify(
                r => r.GetUserIdByAuctionDocumentId(It.IsAny<Guid>()),
                Times.Never
            );
            _repositoryMock.Verify(
                r => r.GetAuctionNameByAuctionDocumentsIdAsync(It.IsAny<Guid>()),
                Times.Never
            );
            _notificationSenderMock.Verify(
                s => s.SendToUsersAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()),
                Times.Never
            );
            _repositoryMock.Verify(
                r => r.SaveNotificationAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()),
                Times.Never
            );
        }
    }
}
