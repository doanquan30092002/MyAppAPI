using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.ChangeStatusEmployeeAccount.Tests
{
    [TestFixture()]
    public class ChangeStatusEmployeeAccountHandlerTests
    {
        private Mock<IEmployeeManagerRepository> _repoMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private ChangeStatusEmployeeAccountHandler _handler;

        private static readonly Guid FixedUserId = Guid.Parse(
            "22222222-2222-2222-2222-222222222222"
        );

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IEmployeeManagerRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new ChangeStatusEmployeeAccountHandler(
                _repoMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Test]
        public void Handle_UserNotLoggedIn_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns((string)null);

            var request = new ChangeStatusEmployeeAccountRequest
            {
                AccountId = Guid.NewGuid(),
                IsActive = true,
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _handler.Handle(request, CancellationToken.None)
            );

            Assert.That(ex.Message, Is.EqualTo("Yêu cầu đăng nhập"));
            _repoMock.Verify(
                r =>
                    r.ChangeStatusEmployeeAccount(
                        It.IsAny<Guid>(),
                        It.IsAny<bool>(),
                        It.IsAny<Guid>()
                    ),
                Times.Never
            );
        }

        [Test]
        public async Task Handle_WhenRepositoryReturnsTrue_ReturnsTrue()
        {
            // Arrange
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(FixedUserId.ToString());

            var accountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            _repoMock
                .Setup(r => r.ChangeStatusEmployeeAccount(accountId, true, FixedUserId))
                .ReturnsAsync(true);

            var request = new ChangeStatusEmployeeAccountRequest
            {
                AccountId = accountId,
                IsActive = true,
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            _repoMock.Verify(
                r => r.ChangeStatusEmployeeAccount(accountId, true, FixedUserId),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_WhenRepositoryReturnsFalse_ReturnsFalse()
        {
            // Arrange
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(FixedUserId.ToString());

            var accountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            _repoMock
                .Setup(r => r.ChangeStatusEmployeeAccount(accountId, false, FixedUserId))
                .ReturnsAsync(false);

            var request = new ChangeStatusEmployeeAccountRequest
            {
                AccountId = accountId,
                IsActive = false,
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _repoMock.Verify(
                r => r.ChangeStatusEmployeeAccount(accountId, false, FixedUserId),
                Times.Once
            );
        }
    }
}
