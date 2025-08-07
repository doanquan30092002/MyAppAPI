using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Sha256Hasher;
using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.UpdateAccount.Command.VerifyAndUpdate.Tests
{
    [TestFixture()]
    public class VerifyAndUpdateHandlerTests
    {
        private Mock<IUpdateAccountRepository> _repoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IOTPService_1> _otpServiceMock;
        private IMemoryCache _cache;
        private VerifyAndUpdateHandler _handler;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IUpdateAccountRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _otpServiceMock = new Mock<IOTPService_1>();

            _cache = new MemoryCache(new MemoryCacheOptions());

            _handler = new VerifyAndUpdateHandler(
                _repoMock.Object,
                _httpContextAccessorMock.Object,
                _otpServiceMock.Object,
                _cache
            );

            _httpContext = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-id") };
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContext);
        }

        [TearDown]
        public void TearDown()
        {
            if (_cache is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [Test]
        public async Task Handle_Unauthorized_Returns401()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);
            var result = await _handler.Handle(
                new VerifyAndUpdateRequest(),
                CancellationToken.None
            );

            Assert.AreEqual(401, result.Code);
            Assert.AreEqual("Unauthorized", result.Message);
        }

        [Test]
        public async Task Handle_VerifyOtpFailed_Returns400()
        {
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((false, "Invalid OTP"));

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual("Invalid OTP", result.Message);
        }

        [Test]
        public async Task Handle_MissingCache_Returns400()
        {
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual(Message.NO_FIELDS_PROVIDED, result.Message);
        }

        [Test]
        public async Task Handle_NoFieldsProvided_Returns400()
        {
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));

            _cache.Set("update_pending_test@example.com", new UpdateAccountRequest());

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual(Message.NO_FIELDS_PROVIDED, result.Message);
        }

        [Test]
        public async Task Handle_AccountNotFound_Returns404()
        {
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));
            _cache.Set("update_pending_test@example.com", new UpdateAccountRequest { Email = "a" });
            _repoMock
                .Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Account)null);

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(404, result.Code);
            Assert.AreEqual(Message.ACCOUNT_NOT_EXSIT, result.Message);
        }

        [Test]
        public async Task Handle_EmailAlreadyUsed_Returns400()
        {
            var acc = new Account { AccountId = Guid.NewGuid(), Password = "hashedpass" };
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));
            _cache.Set(
                "update_pending_test@example.com",
                new UpdateAccountRequest { Email = "EmailAlreadyUsed@example.com" }
            );
            _repoMock.Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(acc);
            _repoMock.Setup(r => r.IsEmailUsedByOtherAsync(acc.AccountId, "a")).ReturnsAsync(true);

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual(Message.EMAIL_EXITS, result.Message);
        }

        [Test]
        public async Task Handle_PhoneAlreadyUsed_Returns400()
        {
            var acc = new Account { AccountId = Guid.NewGuid(), Password = "hashedpass" };
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));
            _cache.Set(
                "update_pending_test@example.com",
                new UpdateAccountRequest { PhoneNumber = "01234" }
            );
            _repoMock.Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(acc);
            _repoMock
                .Setup(r => r.IsPhoneUsedByOtherAsync(acc.AccountId, "01234"))
                .ReturnsAsync(true);

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual(Message.PHONE_NUMBER_EXITS, result.Message);
        }

        [Test]
        public async Task Handle_MissingPassword_Returns400()
        {
            var acc = new Account { AccountId = Guid.NewGuid(), Password = "oldhash" };
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));
            _cache.Set(
                "update_pending_test@example.com",
                new UpdateAccountRequest { PasswordOld = "old" }
            );
            _repoMock.Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(acc);

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual(Message.PASSWORD_OLD_OR_NEW_EMPTY, result.Message);
        }

        [Test]
        public async Task Handle_WrongOldPassword_Returns400()
        {
            var acc = new Account { AccountId = Guid.NewGuid(), Password = "wronghash" };
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));
            _cache.Set(
                "update_pending_test@example.com",
                new UpdateAccountRequest { PasswordOld = "123", PasswordNew = "456" }
            );
            _repoMock.Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(acc);

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(400, result.Code);
            Assert.AreEqual(Message.PASSWORD_OLD_NOT_EQUAL, result.Message);
        }

        [Test]
        public async Task Handle_SuccessfulUpdate_Returns200()
        {
            var acc = new Account
            {
                AccountId = Guid.NewGuid(),
                Password = Sha256Hasher.ComputeSha256Hash("123"),
            };

            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));

            _cache.Set(
                "update_pending_test@example.com",
                new UpdateAccountRequest { PasswordOld = "123", PasswordNew = "456" }
            );

            _repoMock.Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(acc);
            _repoMock.Setup(r => r.UpdateAccountAsync(acc)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(200, result.Code);
            Assert.AreEqual(Message.UPDATE_ACCOUNT_SUCCESS, result.Message);
        }

        [Test]
        public async Task Handle_UpdateThrowsException_Returns500()
        {
            var acc = new Account
            {
                AccountId = Guid.NewGuid(),
                Password = Sha256Hasher.ComputeSha256Hash("123"),
            };

            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));

            _cache.Set(
                "update_pending_test@example.com",
                new UpdateAccountRequest { PasswordOld = "123", PasswordNew = "456" }
            );

            _repoMock.Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(acc);
            _repoMock
                .Setup(r => r.UpdateAccountAsync(acc))
                .ThrowsAsync(new Exception("Something went wrong"));

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(500, result.Code);
            Assert.AreEqual(Message.SYSTEM_ERROR, result.Message);
        }
    }
}
