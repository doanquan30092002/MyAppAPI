using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Notifications.GetNofiticationByUserId.Queries;
using MyApp.Application.CQRS.Notifications.GetNotificationByUserId.Queries;
using MyApp.Application.Interfaces.INotificationsRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Notifications.GetNofiticationByUserId.Queries.Tests
{
    [TestFixture]
    public class GetNotificationsUserIdHandlerTests
    {
        private Mock<INotificationsRepository> _mockRepo;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private GetNotificationsUserIdHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<INotificationsRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _handler = new GetNotificationsUserIdHandler(
                _mockRepo.Object,
                _mockCurrentUserService.Object
            );
        }

        [Test]
        public void Handle_UserIdIsNull_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((string)null);
            var request = new GetNotificationsByUserIdRequest { PageIndex = 1, PageSize = 10 };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_UserIdIsInvalid_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns("invalid-guid");
            var request = new GetNotificationsByUserIdRequest { PageIndex = 1, PageSize = 10 };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_NoNotifications_ReturnsEmptyList()
        {
            var userId = Guid.NewGuid();
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo
                .Setup(x => x.GetNotificationsByUserIdAsync(userId, 1, 10))
                .ReturnsAsync(new List<MyApp.Core.Entities.Notification>());
            _mockRepo.Setup(x => x.GetTotalNotificationsByUserIdAsync(userId)).ReturnsAsync(0);

            var request = new GetNotificationsByUserIdRequest { PageIndex = 1, PageSize = 10 };
            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(1, result.PageIndex);
            Assert.AreEqual(10, result.PageSize);
            Assert.IsEmpty(result.Notifications);
        }

        [Test]
        public async Task Handle_NotificationsExist_ReturnsNotifications()
        {
            var userId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();

            var notificationList = new List<MyApp.Core.Entities.Notification>
            {
                new MyApp.Core.Entities.Notification
                {
                    NotificationId = notificationId,
                    UserId = userId,
                    Message = "Test message",
                    NotificationType = 1,
                    IsRead = false,
                    SentAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UrlAction = "/test",
                },
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo
                .Setup(x => x.GetNotificationsByUserIdAsync(userId, 1, 10))
                .ReturnsAsync(notificationList);
            _mockRepo.Setup(x => x.GetTotalNotificationsByUserIdAsync(userId)).ReturnsAsync(1);

            var request = new GetNotificationsByUserIdRequest { PageIndex = 1, PageSize = 10 };
            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.PageIndex);
            Assert.AreEqual(10, result.PageSize);
            Assert.IsNotEmpty(result.Notifications);

            var dto = result.Notifications[0];
            Assert.AreEqual(notificationId, dto.NotificationId);
            Assert.AreEqual(userId, dto.UserId);
            Assert.AreEqual("Test message", dto.Message);
            Assert.AreEqual(1, dto.NotificationType);
            Assert.IsFalse(dto.IsRead);
            Assert.AreEqual("/test", dto.UrlAction);
        }

        [Test]
        public void Handle_PageIndexOutOfRange_ValidationFails()
        {
            var request = new GetNotificationsByUserIdRequest { PageIndex = 0, PageSize = 10 };
            var context = new ValidationContext(request, null, null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.IsFalse(isValid);
            Assert.AreEqual("PageIndex phải >= 1", results[0].ErrorMessage);
        }

        [Test]
        public void Handle_PageSizeOutOfRange_ValidationFails()
        {
            var request = new GetNotificationsByUserIdRequest { PageIndex = 1, PageSize = 101 };
            var context = new ValidationContext(request, null, null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.IsFalse(isValid);
            Assert.AreEqual("PageSize phải trong khoảng từ 1 đến 100", results[0].ErrorMessage);
        }
    }
}
