using Microsoft.Extensions.Caching.Memory;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;

namespace MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp.Tests
{
    [TestFixture()]
    public class UpdateAccountHandlerTests
    {
        private Mock<IUpdateAccountRepository> _updateAccountRepoMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IOTPService_1> _otpServiceMock;
        private IMemoryCache _memoryCache;
        private UpdateAccountHandler _handler;

        [SetUp]
        public void Setup()
        {
            _updateAccountRepoMock = new Mock<IUpdateAccountRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _otpServiceMock = new Mock<IOTPService_1>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _handler = new UpdateAccountHandler(
                _updateAccountRepoMock.Object,
                _currentUserServiceMock.Object,
                _otpServiceMock.Object,
                _memoryCache
            );
        }

        [TearDown]
        public void TearDown()
        {
            if (_memoryCache is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [Test]
        public async Task Handle_ReturnsTrue_WhenOtpSentSuccessfully()
        {
            // Arrange
            var userId = "test-user-id";
            var email = "test@example.com";
            var request = new UpdateAccountRequest();

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);
            _updateAccountRepoMock.Setup(r => r.GetEmailByUserIdAsync(userId)).ReturnsAsync(email);
            _otpServiceMock.Setup(s => s.SendOtpAsync(email)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_memoryCache.TryGetValue($"update_pending_{email}", out _));
            _otpServiceMock.Verify(s => s.SendOtpAsync(email), Times.Once);
        }

        [Test]
        public void Handle_ThrowsException_WhenUserIdIsMissing()
        {
            // Arrange
            var request = new UpdateAccountRequest();
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns((string?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _handler.Handle(request, default)
            );
            Assert.That(ex!.Message, Is.EqualTo("Yêu cầu đăng nhập"));
        }

        [Test]
        public async Task Handle_ReturnsFalse_WhenOtpSendFails()
        {
            // Arrange
            var userId = "test-user-id";
            var email = "test@example.com";
            var request = new UpdateAccountRequest();

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);
            _updateAccountRepoMock.Setup(r => r.GetEmailByUserIdAsync(userId)).ReturnsAsync(email);
            _otpServiceMock.Setup(s => s.SendOtpAsync(email)).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
