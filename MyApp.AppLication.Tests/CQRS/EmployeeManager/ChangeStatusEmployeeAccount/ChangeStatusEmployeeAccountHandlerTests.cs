using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.ChangeStatusEmployeeAccount.Tests
{
    [TestFixture()]
    public class ChangeStatusEmployeeAccountHandlerTests
    {
        private Mock<IEmployeeManagerRepository> _repoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private ChangeStatusEmployeeAccountHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IEmployeeManagerRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _handler = new ChangeStatusEmployeeAccountHandler(
                _repoMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Test]
        public async Task Handle_WhenRepositoryReturnsTrue_ReturnsTrue()
        {
            // Arrange
            var accountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var request = new ChangeStatusEmployeeAccountRequest
            {
                AccountId = accountId,
                IsActive = true,
            };

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }
                )
            );

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

            _repoMock
                .Setup(r => r.ChangeStatusEmployeeAccount(accountId, true, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            _repoMock.Verify(
                r => r.ChangeStatusEmployeeAccount(accountId, true, userId),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_WhenRepositoryReturnsFalse_ReturnsFalse()
        {
            // Arrange
            var accountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var userId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var request = new ChangeStatusEmployeeAccountRequest
            {
                AccountId = accountId,
                IsActive = false,
            };

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }
                )
            );

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

            _repoMock
                .Setup(r => r.ChangeStatusEmployeeAccount(accountId, false, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _repoMock.Verify(
                r => r.ChangeStatusEmployeeAccount(accountId, false, userId),
                Times.Once
            );
        }
    }
}
