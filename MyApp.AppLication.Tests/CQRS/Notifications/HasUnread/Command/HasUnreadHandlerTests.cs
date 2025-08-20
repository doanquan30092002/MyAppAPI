using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Notifications.HasUnread.Command;
using MyApp.Application.Interfaces.INotificationsRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Notifications.HasUnread.Command.Tests
{
    [TestFixture]
    public class HasUnreadHandlerTests
    {
        private Mock<INotificationsRepository> _mockRepo;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private HasUnreadHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<INotificationsRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _handler = new HasUnreadHandler(_mockRepo.Object, _mockCurrentUserService.Object);
        }

        [Test]
        public void Handle_UserIdIsNull_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((string)null);
            var command = new HasUnreadCommand();

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_UserIdIsInvalid_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns("invalid-guid");
            var command = new HasUnreadCommand();

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_HasUnread_ReturnsTrue()
        {
            var userId = Guid.NewGuid();
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo.Setup(x => x.HasUnreadNotificationAsync(userId)).ReturnsAsync(true);

            var command = new HasUnreadCommand();
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Handle_NoUnread_ReturnsFalse()
        {
            var userId = Guid.NewGuid();
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo.Setup(x => x.HasUnreadNotificationAsync(userId)).ReturnsAsync(false);

            var command = new HasUnreadCommand();
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result);
        }
    }
}
