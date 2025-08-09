using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.UpdateStatusTicket.Tests
{
    [TestFixture()]
    public class UpdateStatusTicketHandlerTests
    {
        private Mock<IRegisterAuctionDocumentRepository> _repositoryMock;
        private Mock<INotificationSender> _notificationSenderMock;
        private UpdateStatusTicketHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IRegisterAuctionDocumentRepository>();
            _notificationSenderMock = new Mock<INotificationSender>();

            _handler = new UpdateStatusTicketHandler(
                _repositoryMock.Object,
                _notificationSenderMock.Object
            );
        }

        [Test]
        public async Task Handle_UpdateSuccess_ShouldSendAndSaveNotification()
        {
            // Arrange
            var auctionDocId = Guid.Parse(" ");
            var staffIds = new List<Guid>
            {
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            };
            var auctionName = "Đấu giá xe hơi";

            _repositoryMock
                .Setup(r => r.UpdateStatusTicketAndGetUserIdAsync(auctionDocId))
                .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.GetUserIdByRoleAsync()).ReturnsAsync(staffIds);
            _repositoryMock
                .Setup(r => r.GetAuctionNameByAuctionDocumentsIdAsync(auctionDocId))
                .ReturnsAsync(auctionName);

            var request = new UpdateStatusTicketRequest { AuctionDocumentsId = auctionDocId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            _notificationSenderMock.Verify(
                n =>
                    n.SendToUsersAsync(
                        staffIds,
                        string.Format(Message.NEW_AUCTION_TO_CUSTOMER, auctionName)
                    ),
                Times.Once
            );
            _repositoryMock.Verify(
                r =>
                    r.SaveNotificationAsync(
                        staffIds,
                        string.Format(Message.NEW_AUCTION_TO_CUSTOMER, auctionName)
                    ),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_UpdateFail_ShouldNotSendOrSaveNotification()
        {
            // Arrange
            var auctionDocId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

            _repositoryMock
                .Setup(r => r.UpdateStatusTicketAndGetUserIdAsync(auctionDocId))
                .ReturnsAsync(false);

            var request = new UpdateStatusTicketRequest { AuctionDocumentsId = auctionDocId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
            _notificationSenderMock.Verify(
                n => n.SendToUsersAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()),
                Times.Never
            );
            _repositoryMock.Verify(
                r => r.SaveNotificationAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()),
                Times.Never
            );
        }
    }
}
