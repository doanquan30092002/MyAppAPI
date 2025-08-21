using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Notifications.MarkAllAsRead;
using MyApp.Application.Interfaces.INotificationsRepository;
using NUnit.Framework;

namespace MyApp.Application.CQRS.Notifications.MarkAllAsRead.Tests
{
    [TestFixture]
    public class MarkAllAsReadHandlerTests
    {
        private Mock<INotificationsRepository> _mockRepo;
        private Mock<ICurrentUserService> _mockCurrentUserService;
        private MarkAllAsReadHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<INotificationsRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _handler = new MarkAllAsReadHandler(_mockRepo.Object, _mockCurrentUserService.Object);
        }

        [Test]
        public void Handle_UserIdIsNull_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((string)null);
            var request = new MarkAllAsReadRequest();

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public void Handle_UserIdIsInvalid_ThrowsUnauthorizedAccessException()
        {
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns("invalid-guid");
            var request = new MarkAllAsReadRequest();

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _handler.Handle(request, CancellationToken.None)
            );
        }

        [Test]
        public async Task Handle_Success_ReturnsUpdatedCount()
        {
            var userId = Guid.NewGuid();
            int updatedCount = 5;

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId.ToString());
            _mockRepo.Setup(x => x.MarkAllAsReadAsync(userId)).ReturnsAsync(updatedCount);

            var request = new MarkAllAsReadRequest();
            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(updatedCount, result);
        }
    }
}
