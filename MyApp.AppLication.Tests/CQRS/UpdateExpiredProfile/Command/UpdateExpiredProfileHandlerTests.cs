using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.UpdateExpiredProfile;

namespace MyApp.Application.CQRS.UpdateExpiredProfile.Command.Tests
{
    [TestFixture()]
    public class UpdateExpiredProfileHandlerTests
    {
        private Mock<IUpdateExpiredProfileRepository> _repositoryMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private UpdateExpiredProfileHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUpdateExpiredProfileRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new UpdateExpiredProfileHandler(
                _repositoryMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturn401_WhenUserIdIsMissing()
        {
            // Arrange
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns((string)null);

            var request = new UpdateExpiredProfileRequest
            {
                CitizenIdentification = "123456789",
                Name = "Nguyễn Văn A",
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(401));
            Assert.That(result.Message, Is.EqualTo(Message.LOGIN_INFO_NOT_FOUND));
        }

        [Test]
        public async Task Handle_ShouldReturn404_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);
            _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((UserResponse)null);

            var request = new UpdateExpiredProfileRequest
            {
                CitizenIdentification = "0123456789",
                Name = "Nguyễn Văn B",
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(404));
            Assert.That(result.Message, Is.EqualTo(Message.USER_DOES_NOT_EXSIT));
        }

        [Test]
        public async Task Handle_ShouldReturn400_WhenCitizenIdentificationNotMatch()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);

            var existingUser = new UserResponse { CitizenIdentification = "987654321" };
            _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);

            var request = new UpdateExpiredProfileRequest
            {
                CitizenIdentification = "123456789",
                Name = "Nguyễn Văn C",
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo(Message.CITIZEN_IDENTIFICATION_NOT_MATCH));
        }

        [Test]
        public async Task Handle_ShouldReturn200_WhenUpdateSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);

            var existingUser = new UserResponse { CitizenIdentification = "123456789" };
            _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);

            var request = new UpdateExpiredProfileRequest
            {
                CitizenIdentification = "123456789",
                Name = "Nguyễn Văn D",
                BirthDay = new DateTime(2000, 1, 1),
                Nationality = "Việt Nam",
                Gender = true,
                ValidDate = new DateTime(2035, 1, 1),
                OriginLocation = "Hà Nội",
                RecentLocation = "TP. Hồ Chí Minh",
                IssueDate = new DateTime(2025, 1, 1),
                IssueBy = "Cục Cảnh sát QLHC về TTXH",
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo(Message.UPDATE_PROFILE_SUCCESS));
            _repositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<UserResponse>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturn500_WhenExceptionThrown()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);

            var existingUser = new UserResponse { CitizenIdentification = "123456789" };
            _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);

            _repositoryMock
                .Setup(r => r.UpdateUserAsync(It.IsAny<UserResponse>()))
                .ThrowsAsync(new Exception("Lỗi hệ thống"));

            var request = new UpdateExpiredProfileRequest
            {
                CitizenIdentification = "123456789",
                Name = "Nguyễn Văn D",
                BirthDay = new DateTime(2000, 1, 1),
                Nationality = "Việt Nam",
                Gender = true,
                ValidDate = new DateTime(2035, 1, 1),
                OriginLocation = "Hà Nội",
                RecentLocation = "TP. Hồ Chí Minh",
                IssueDate = new DateTime(2025, 1, 1),
                IssueBy = "Cục Cảnh sát QLHC về TTXH",
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(500));
            Assert.That(result.Message, Is.EqualTo(Message.SYSTEM_ERROR));
        }
    }
}
