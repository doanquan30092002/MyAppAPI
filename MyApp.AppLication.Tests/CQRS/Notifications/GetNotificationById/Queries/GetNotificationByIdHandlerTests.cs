using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Notifications.GetNotificationById.Queries;
using MyApp.Application.Interfaces.INotificationsRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Notifications.GetNotificationById.Queries.Tests
{
    [TestFixture]
    public class GetNotificationByIdHandlerTests
    {
        private Mock<INotificationsRepository> _mockRepo;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private GetNotificationByIdHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<INotificationsRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _handler = new GetNotificationByIdHandler(
                _mockRepo.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public void Handle_UserIdIsNull_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((string)null);

            var request = new GetNotificationsByIdRequest { NotificationId = Guid.NewGuid() };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_UserIdIsInvalid_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns("invalid-guid");

            var request = new GetNotificationsByIdRequest { NotificationId = Guid.NewGuid() };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_NotificationNotFound_ThrowsKeyNotFoundException()
        {
            var userId = Guid.NewGuid();
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo
                .Setup(x => x.GetNotificationByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((MyApp.Core.Entities.Notification)null);

            var request = new GetNotificationsByIdRequest { NotificationId = Guid.NewGuid() };

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_NotificationBelongsToOtherUser_ThrowsUnauthorizedAccessException()
        {
            var userId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo
                .Setup(x => x.GetNotificationByIdAsync(notificationId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Notification
                    {
                        NotificationId = notificationId,
                        UserId = Guid.NewGuid(), // khác user hiện tại
                        Message = "Test",
                        NotificationType = 1,
                        IsRead = false,
                        SentAt = DateTime.Now,
                    }
                );

            var request = new GetNotificationsByIdRequest { NotificationId = notificationId };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsNotificationDto()
        {
            var userId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo
                .Setup(x => x.GetNotificationByIdAsync(notificationId))
                .ReturnsAsync(
                    new MyApp.Core.Entities.Notification
                    {
                        NotificationId = notificationId,
                        UserId = userId,
                        Message = "Test",
                        NotificationType = 1,
                        IsRead = false,
                        SentAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        UrlAction = "/test",
                    }
                );

            var request = new GetNotificationsByIdRequest { NotificationId = notificationId };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(notificationId, result.NotificationId);
            Assert.AreEqual(userId, result.UserId);
            Assert.AreEqual("Test", result.Message);
            Assert.AreEqual(1, result.NotificationType);
            Assert.IsFalse(result.IsRead);
            Assert.AreEqual("/test", result.UrlAction);
        }
    }
}
