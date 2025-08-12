using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.AssignPermissionUser.Tests
{
    [TestFixture()]
    public class AssignPermissionUserHandlerTests
    {
        private Mock<IEmployeeManagerRepository> _repoMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private AssignPermissionUserHandler _handler;

        private static readonly Guid FixedUserId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IEmployeeManagerRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new AssignPermissionUserHandler(
                _repoMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Test]
        public void Handle_UserNotLoggedIn_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.GetUserId()).Returns((string)null);

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
            _repoMock.Verify(
                r => r.AssignPermissionUser(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid>()),
                Times.Never
            );
        }

        [Test]
        public async Task Handle_AssignSuccess_ReturnsTrue()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(FixedUserId.ToString());

            var accountId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            _repoMock
                .Setup(r => r.AssignPermissionUser(accountId, 3, FixedUserId))
                .ReturnsAsync(true);

            var request = new AssignPermissionUserRequest { AccountId = accountId, RoleId = 3 };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            _repoMock.Verify(r => r.AssignPermissionUser(accountId, 3, FixedUserId), Times.Once);
        }

        [Test]
        public async Task Handle_AssignFails_ReturnsFalse()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(FixedUserId.ToString());

            var accountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            _repoMock
                .Setup(r => r.AssignPermissionUser(accountId, 5, FixedUserId))
                .ReturnsAsync(false);

            var request = new AssignPermissionUserRequest { AccountId = accountId, RoleId = 5 };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _repoMock.Verify(r => r.AssignPermissionUser(accountId, 5, FixedUserId), Times.Once);
        }
    }
}
