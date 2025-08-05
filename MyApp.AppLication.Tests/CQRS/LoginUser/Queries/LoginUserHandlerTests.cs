using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Core.DTOs.LoginUserDTO;

namespace MyApp.Application.CQRS.LoginUser.Queries.Tests
{
    [TestFixture]
    public class LoginUserHandlerTests
    {
        private Mock<ILoginUserRepository> _loginUserRepoMock;
        private Mock<ITokenRepository> _tokenRepoMock;
        private LoginUserHandler _handler;

        [SetUp]
        public void Setup()
        {
            _loginUserRepoMock = new Mock<ILoginUserRepository>();
            _tokenRepoMock = new Mock<ITokenRepository>();
            _handler = new LoginUserHandler(_loginUserRepoMock.Object, _tokenRepoMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnWrongMessage_WhenAccountIsNull()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "user@example.com",
                Password = "password",
            };
            _loginUserRepoMock
                .Setup(x => x.GetAccountLogin(request.Email, It.IsAny<string>()))
                .ReturnsAsync((AccountDTO)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Token, Is.Null);
            Assert.That(result.Message, Is.EqualTo(Message.LOGIN_WRONG));
        }

        [Test]
        public async Task Handle_ShouldReturnAccountLockedMessage_WhenAccountIsInactive()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "locked@example.com",
                Password = "password",
            };
            var account = new AccountDTO { Email = request.Email, IsActive = false };
            _loginUserRepoMock
                .Setup(x => x.GetAccountLogin(request.Email, It.IsAny<string>()))
                .ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Token, Is.Null);
            Assert.That(result.Message, Is.EqualTo(Message.ACCOUNT_LOCKED));
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenAccountValid_ValidDateInFuture()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "user@example.com",
                Password = "password",
            };
            var account = new AccountDTO { Email = request.Email, IsActive = true };
            var user = new UserDTO
            {
                Id = Guid.Parse("68FAA0B5-2E6B-4792-8CEB-8A5E951A3E1A"),
                Name = "Test User",
                ValidDate = DateTime.UtcNow.AddDays(1),
            };
            var role = "Admin";
            var token = "mock-token";

            _loginUserRepoMock
                .Setup(x => x.GetAccountLogin(request.Email, It.IsAny<string>()))
                .ReturnsAsync(account);

            _loginUserRepoMock.Setup(x => x.GetUserByEmail(request.Email)).ReturnsAsync(user);
            _loginUserRepoMock.Setup(x => x.GetRoleNameByEmail(request.Email)).ReturnsAsync(role);
            _tokenRepoMock.Setup(x => x.CreateJWTToken(user, role)).Returns(token);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Token, Is.EqualTo(token));
            Assert.That(result.Message, Is.EqualTo(Message.LOGIN_SUCCESS));
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Email, Is.EqualTo(account.Email));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.RoleName, Is.EqualTo(role));
        }

        [Test]
        public async Task Handle_ShouldReturnExpiredMessage_WhenValidDateExpired()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "user@example.com",
                Password = "password",
            };
            var account = new AccountDTO { Email = request.Email, IsActive = true };
            var user = new UserDTO
            {
                Id = Guid.Parse("68FAA0B5-2E6B-4792-8CEB-8A5E951A3E1A"),
                Name = "Expired User",
                ValidDate = DateTime.UtcNow.AddDays(-1),
            };
            var role = "User";
            var token = "expired-token";

            _loginUserRepoMock
                .Setup(x => x.GetAccountLogin(request.Email, It.IsAny<string>()))
                .ReturnsAsync(account);

            _loginUserRepoMock.Setup(x => x.GetUserByEmail(request.Email)).ReturnsAsync(user);
            _loginUserRepoMock.Setup(x => x.GetRoleNameByEmail(request.Email)).ReturnsAsync(role);
            _tokenRepoMock.Setup(x => x.CreateJWTToken(user, role)).Returns(token);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Token, Is.EqualTo(token));
            Assert.That(result.Message, Is.EqualTo(Message.EXPIRED_CITIZEN_IDENTIFICATION));
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Email, Is.EqualTo(account.Email));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.RoleName, Is.EqualTo(role));
        }
    }
}
