using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Notifications.MarkAsRead.Commands;
using MyApp.Application.Interfaces.INotificationsRepository;
using MyApp.Core.Entities;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Notifications.MarkAsRead.Commands.Tests
{
    [TestFixture]
    public class MarkAsReadHandlerTests
    {
        private Mock<INotificationsRepository> _mockRepo;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private MarkAsReadHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<INotificationsRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _handler = new MarkAsReadHandler(_mockRepo.Object, _mockCurrentUserService.Object);
        }

        [Test]
        public void Handle_UserIdIsNull_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((string)null);
            var request = new MarkAsReadRequest(Guid.NewGuid());

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_UserIdIsInvalid_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns("invalid-guid");
            var request = new MarkAsReadRequest(Guid.NewGuid());

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
                .ReturnsAsync((Notification)null);

            var request = new MarkAsReadRequest(Guid.NewGuid());

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_NotificationBelongsToAnotherUser_ThrowsUnauthorizedAccessException()
        {
            var userId = Guid.NewGuid();
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // khác user hiện tại
                Message = "Test",
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo
                .Setup(x => x.GetNotificationByIdAsync(notification.NotificationId))
                .ReturnsAsync(notification);

            var request = new MarkAsReadRequest(notification.NotificationId);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_ValidNotification_ReturnsTrue()
        {
            var userId = Guid.NewGuid();
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = userId,
                Message = "Test",
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo
                .Setup(x => x.GetNotificationByIdAsync(notification.NotificationId))
                .ReturnsAsync(notification);
            _mockRepo.Setup(x => x.MarkAsReadAsync(notification.NotificationId)).ReturnsAsync(true);

            var request = new MarkAsReadRequest(notification.NotificationId);
            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Handle_ValidNotification_ReturnsFalse()
        {
            var userId = Guid.NewGuid();
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = userId,
                Message = "Test",
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo
                .Setup(x => x.GetNotificationByIdAsync(notification.NotificationId))
                .ReturnsAsync(notification);
            _mockRepo
                .Setup(x => x.MarkAsReadAsync(notification.NotificationId))
                .ReturnsAsync(false);

            var request = new MarkAsReadRequest(notification.NotificationId);
            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsFalse(result);
        }
    }
}
