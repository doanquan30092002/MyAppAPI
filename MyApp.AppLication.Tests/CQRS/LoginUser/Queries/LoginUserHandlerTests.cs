using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Sha256Hasher;
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
        public async Task Handle_ShouldReturnSuccess_WhenValidCredentialsAndActiveAndValidDate()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "test@example.com",
                Password = "password123",
            };

            var hashedPassword = Sha256Hasher.ComputeSha256Hash(request.Password);

            var account = new AccountDTO { Email = request.Email, IsActive = true };

            var user = new UserDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                ValidDate = DateTime.UtcNow.AddDays(5),
            };

            var role = "Admin";
            var token = "mocked_jwt_token";

            _loginUserRepoMock
                .Setup(x => x.GetAccountLogin(request.Email, hashedPassword))
                .ReturnsAsync(account);

            _loginUserRepoMock.Setup(x => x.GetUserByEmail(account.Email)).ReturnsAsync(user);

            _loginUserRepoMock.Setup(x => x.GetRoleNameByEmail(account.Email)).ReturnsAsync(role);

            _tokenRepoMock.Setup(x => x.CreateJWTToken(user, role)).Returns(token);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Token, Is.EqualTo(token));
            Assert.That(result.Message, Is.EqualTo(Message.LOGIN_SUCCESS));
            Assert.That(result.Email, Is.EqualTo(account.Email));
            Assert.That(result.RoleName, Is.EqualTo(role));
        }

        [Test]
        public async Task Handle_ShouldReturnAccountLocked_WhenAccountInactive()
        {
            var request = new LoginUserRequest { Email = "test@example.com", Password = "123" };
            var hashed = Sha256Hasher.ComputeSha256Hash(request.Password);

            _loginUserRepoMock
                .Setup(x => x.GetAccountLogin(request.Email, hashed))
                .ReturnsAsync(new AccountDTO { Email = request.Email, IsActive = false });

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Token, Is.Null);
            Assert.That(result.Message, Is.EqualTo(Message.ACCOUNT_LOCKED));
        }

        [Test]
        public async Task Handle_ShouldReturnExpired_WhenValidDateExpired()
        {
            var request = new LoginUserRequest { Email = "test@example.com", Password = "123" };
            var hashed = Sha256Hasher.ComputeSha256Hash(request.Password);

            _loginUserRepoMock
                .Setup(x => x.GetAccountLogin(request.Email, hashed))
                .ReturnsAsync(new AccountDTO { Email = request.Email, IsActive = true });

            _loginUserRepoMock
                .Setup(x => x.GetUserByEmail(request.Email))
                .ReturnsAsync(new UserDTO { ValidDate = DateTime.UtcNow.AddDays(-1) });

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Token, Is.Null);
            Assert.That(result.Message, Is.EqualTo(Message.EXPIRED_CITIZEN_IDENTIFICATION));
        }

        [Test]
        public async Task Handle_ShouldReturnWrongCredentials_WhenAccountNotFound()
        {
            var request = new LoginUserRequest
            {
                Email = "wrong@example.com",
                Password = "badpass",
            };
            var hashed = Sha256Hasher.ComputeSha256Hash(request.Password);

            _loginUserRepoMock
                .Setup(x => x.GetAccountLogin(request.Email, hashed))
                .ReturnsAsync((AccountDTO)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Token, Is.Null);
            Assert.That(result.Message, Is.EqualTo(Message.LOGIN_WRONG));
        }
    }
}
