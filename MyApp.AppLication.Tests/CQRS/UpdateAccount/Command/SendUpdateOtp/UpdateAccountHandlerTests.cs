using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;

namespace MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp.Tests
{
    [TestFixture()]
    public class UpdateAccountHandlerTests
    {
        private Mock<IUpdateAccountRepository> _updateAccountRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IOTPService_1> _otpServiceMock;
        private IMemoryCache _memoryCache;
        private Mock<ICacheEntry> _cacheEntryMock;
        private UpdateAccountHandler _handler;

        [SetUp]
        public void Setup()
        {
            _updateAccountRepoMock = new Mock<IUpdateAccountRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _otpServiceMock = new Mock<IOTPService_1>();

            (_memoryCache, _cacheEntryMock) = CreateMockMemoryCache();

            _handler = new UpdateAccountHandler(
                _updateAccountRepoMock.Object,
                _httpContextAccessorMock.Object,
                _otpServiceMock.Object,
                _memoryCache
            );
        }

        private (IMemoryCache, Mock<ICacheEntry>) CreateMockMemoryCache()
        {
            var cacheEntryMock = new Mock<ICacheEntry>();
            cacheEntryMock.SetupAllProperties();

            var memoryCacheMock = new Mock<IMemoryCache>();
            memoryCacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            // Optional: TryGetValue can be set to return false
            object dummy;
            memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out dummy)).Returns(false);

            return (memoryCacheMock.Object, cacheEntryMock);
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

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User).Returns(principal);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _updateAccountRepoMock.Setup(r => r.GetEmailByUserIdAsync(userId)).ReturnsAsync(email);
            _otpServiceMock.Setup(s => s.SendOtpAsync(email)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            _otpServiceMock.Verify(s => s.SendOtpAsync(email), Times.Once);
        }

        [Test]
        public void Handle_ThrowsException_WhenUserIdIsMissing()
        {
            // Arrange
            var request = new UpdateAccountRequest();

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(new ClaimsPrincipal()); // Không có claim

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _handler.Handle(request, default)
            );
            Assert.That(ex.Message, Is.EqualTo("User not authenticated."));
        }

        [Test]
        public async Task Handle_ReturnsFalse_WhenOtpSendFails()
        {
            // Arrange
            var userId = "test-user-id";
            var email = "test@example.com";
            var request = new UpdateAccountRequest();

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User).Returns(principal);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _updateAccountRepoMock.Setup(r => r.GetEmailByUserIdAsync(userId)).ReturnsAsync(email);
            _otpServiceMock.Setup(s => s.SendOtpAsync(email)).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
