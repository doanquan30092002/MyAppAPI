using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.AssignPermissionUser.Tests
{
    [TestFixture()]
    public class AssignPermissionUserHandlerTests
    {
        private Mock<IEmployeeManagerRepository> _repoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private AssignPermissionUserHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IEmployeeManagerRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _handler = new AssignPermissionUserHandler(
                _repoMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        private void SetupHttpContextWithUser(Guid userId)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        [Test]
        public void Handle_UserNotLoggedIn_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()); // Không có NameIdentifier claim
            var request = new AssignPermissionUserRequest
            {
                AccountId = Guid.NewGuid(),
                RoleId = 1,
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _handler.Handle(request, CancellationToken.None)
            );

            Assert.That(ex.Message, Is.EqualTo("Yêu cầu đăng nhập"));
        }

        [Test]
        public async Task Handle_AssignSuccess_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            SetupHttpContextWithUser(userId);

            var accountId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            _repoMock.Setup(x => x.AssignPermissionUser(accountId, 3, userId)).ReturnsAsync(true);

            var request = new AssignPermissionUserRequest { AccountId = accountId, RoleId = 3 };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            _repoMock.Verify(x => x.AssignPermissionUser(accountId, 3, userId), Times.Once);
        }

        [Test]
        public async Task Handle_AssignFails_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            SetupHttpContextWithUser(userId);

            var accountId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            _repoMock.Setup(x => x.AssignPermissionUser(accountId, 5, userId)).ReturnsAsync(false);

            var request = new AssignPermissionUserRequest { AccountId = accountId, RoleId = 5 };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _repoMock.Verify(x => x.AssignPermissionUser(accountId, 5, userId), Times.Once);
        }
    }
}
