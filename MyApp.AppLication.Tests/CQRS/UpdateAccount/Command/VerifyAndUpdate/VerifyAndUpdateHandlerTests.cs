using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;

namespace MyApp.Application.CQRS.UpdateAccount.Command.VerifyAndUpdate.Tests
{
    [TestFixture()]
    public class VerifyAndUpdateHandlerTests
    {
        private Mock<IUpdateAccountRepository> _updateAccountRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IOTPService_1> _otpServiceMock;
        private IMemoryCache _memoryCache;

        private VerifyAndUpdateHandler _handler;

        private const string TestUserId = "test-user-id";
        private const string TestEmail = "test@example.com";
        private const string _otpCode = "123456";

        [SetUp]
        public void SetUp()
        {
            _updateAccountRepoMock = new Mock<IUpdateAccountRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _otpServiceMock = new Mock<IOTPService_1>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Fake HttpContext with User
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, TestUserId) },
                    "mock"
                )
            );

            var context = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

            _handler = new VerifyAndUpdateHandler(
                _updateAccountRepoMock.Object,
                _httpContextAccessorMock.Object,
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
        public async Task Handle_InvalidOtp_ReturnsError()
        {
            // Arrange
            _updateAccountRepoMock
                .Setup(r => r.GetEmailByUserIdAsync(TestUserId))
                .ReturnsAsync(TestEmail);

            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync(TestEmail, _otpCode))
                .ReturnsAsync((false, "OTP không hợp lệ."));

            var request = new VerifyAndUpdateRequest { OtpCode = _otpCode };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo("OTP không hợp lệ."));
        }

        [Test]
        public async Task Handle_MissingCache_ReturnsError()
        {
            // Arrange
            _updateAccountRepoMock
                .Setup(r => r.GetEmailByUserIdAsync(TestUserId))
                .ReturnsAsync(TestEmail);

            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync(TestEmail, _otpCode))
                .ReturnsAsync((true, "Thành công."));

            var request = new VerifyAndUpdateRequest { OtpCode = _otpCode };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy dữ liệu cập nhật."));
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            _updateAccountRepoMock
                .Setup(r => r.GetEmailByUserIdAsync(TestUserId))
                .ReturnsAsync(TestEmail);

            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync(TestEmail, _otpCode))
                .ReturnsAsync((true, "Thành công."));

            var updateRequest = new UpdateAccountRequest
            {
                Email = "new@example.com",
                PasswordOld = "oldpass",
                PasswordNew = "newpass",
                PhoneNumber = "123456789",
            };

            _memoryCache.Set($"update_pending_{TestEmail}", updateRequest);

            _updateAccountRepoMock
                .Setup(r =>
                    r.UpdateAccountInfo(
                        TestUserId,
                        updateRequest.Email,
                        updateRequest.PasswordOld,
                        updateRequest.PasswordNew,
                        updateRequest.PhoneNumber
                    )
                )
                .ReturnsAsync(
                    new UpdateAccountResponse { Code = 200, Message = "Cập nhật thành công." }
                );

            var request = new VerifyAndUpdateRequest { OtpCode = _otpCode };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Cập nhật thành công."));

            // Cache phải bị remove sau khi cập nhật
            var found = _memoryCache.TryGetValue(
                $"update_pending_{TestEmail}",
                out UpdateAccountRequest _
            );
            Assert.False(found);
        }
    }
}
