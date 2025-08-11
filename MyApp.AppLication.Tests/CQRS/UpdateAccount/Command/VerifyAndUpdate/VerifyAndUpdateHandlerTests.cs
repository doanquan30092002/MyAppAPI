using Microsoft.Extensions.Caching.Memory;
using Moq;
using MyApp.Application.Common.CurrentUserService;
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
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IOTPService_1> _otpServiceMock;
        private IMemoryCache _cache;
        private VerifyAndUpdateHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IUpdateAccountRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _otpServiceMock = new Mock<IOTPService_1>();
            _cache = new MemoryCache(new MemoryCacheOptions());

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns("test-user-id"); // ✅ Mock user id

            _handler = new VerifyAndUpdateHandler(
                _repoMock.Object,
                _currentUserServiceMock.Object, // ✅ Đúng interface
                _otpServiceMock.Object,
                _cache
            );
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
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns((string?)null);
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
            var acc = new Account
            {
                AccountId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Password = "hashedpass",
            };

            // Email trả về từ repo và cache phải trùng nhau
            string email = "EmailAlreadyUsed@example.com";

            _repoMock.Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>())).ReturnsAsync(email);

            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync(email, It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));

            _cache.Set($"update_pending_{email}", new UpdateAccountRequest { Email = email });

            _repoMock.Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(acc);

            _repoMock
                .Setup(r => r.IsEmailUsedByOtherAsync(acc.AccountId, email))
                .ReturnsAsync(true);

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

        [Test]
        public async Task Handle_EmptyUserId_Returns401()
        {
            // Arrange: UserId empty string
            var accServiceMock = new Mock<ICurrentUserService>();
            accServiceMock.Setup(s => s.GetUserId()).Returns(string.Empty);
            var handler = new VerifyAndUpdateHandler(
                _repoMock.Object,
                accServiceMock.Object,
                _otpServiceMock.Object,
                _cache
            );

            // Act
            var result = await handler.Handle(new VerifyAndUpdateRequest(), CancellationToken.None);

            // Assert
            Assert.AreEqual(401, result.Code);
            Assert.AreEqual("Unauthorized", result.Message);
        }

        [Test]
        public async Task Handle_PasswordNewOnly_Returns400()
        {
            var acc = new Account { AccountId = Guid.NewGuid(), Password = "hash" };
            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));
            _cache.Set(
                "update_pending_test@example.com",
                new UpdateAccountRequest { PasswordNew = "newpass" }
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
        public async Task Handle_UpdateEmailAndPhone_Success()
        {
            var acc = new Account
            {
                AccountId = Guid.NewGuid(),
                Password = Sha256Hasher.ComputeSha256Hash("123"),
                Email = "old@example.com",
                PhoneNumber = "0999999999",
            };

            _repoMock
                .Setup(r => r.GetEmailByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync("test@example.com");
            _otpServiceMock
                .Setup(o => o.VerifyOtpAsync("test@example.com", It.IsAny<string>()))
                .ReturnsAsync((true, "OK"));

            _cache.Set(
                "update_pending_test@example.com",
                new UpdateAccountRequest
                {
                    Email = "new@example.com",
                    PhoneNumber = "0888888888",
                    PasswordOld = "123",
                    PasswordNew = "456",
                }
            );

            _repoMock.Setup(r => r.GetAccountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(acc);
            _repoMock
                .Setup(r => r.IsEmailUsedByOtherAsync(acc.AccountId, "new@example.com"))
                .ReturnsAsync(false);
            _repoMock
                .Setup(r => r.IsPhoneUsedByOtherAsync(acc.AccountId, "0888888888"))
                .ReturnsAsync(false);
            _repoMock.Setup(r => r.UpdateAccountAsync(acc)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(
                new VerifyAndUpdateRequest { OtpCode = "1234" },
                CancellationToken.None
            );

            Assert.AreEqual(200, result.Code);
            Assert.AreEqual(Message.UPDATE_ACCOUNT_SUCCESS, result.Message);
            Assert.AreEqual("new@example.com", acc.Email);
            Assert.AreEqual("0888888888", acc.PhoneNumber);
            Assert.AreEqual(Sha256Hasher.ComputeSha256Hash("456"), acc.Password);
        }
    }
}
