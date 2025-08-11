using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.ChangeStatusBlog.Tests
{
    [TestFixture]
    public class ChangeStatusBlogHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private ChangeStatusBlogHandler _handler;

        private readonly Guid _fixedBlogId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private readonly string _fixedUserId = "22222222-2222-2222-2222-222222222222";

        [SetUp]
        public void Setup()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new ChangeStatusBlogHandler(
                _blogRepoMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_WhenChangeSuccess_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new ChangeStatusBlogRequest
            {
                BlogId = _fixedBlogId,
                Status = 1,
                Note = "Đã duyệt",
            };

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(_fixedUserId);
            _blogRepoMock
                .Setup(r => r.ChangeStatusBlogAsync(_fixedBlogId, 1, "Đã duyệt", _fixedUserId))
                .ReturnsAsync(true);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo(Message.UPDATE_STATUS_BLOG_SUCCESS));
            _blogRepoMock.Verify(
                r => r.ChangeStatusBlogAsync(_fixedBlogId, 1, "Đã duyệt", _fixedUserId),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_WhenChangeFails_ReturnsFailResponse()
        {
            // Arrange
            var request = new ChangeStatusBlogRequest
            {
                BlogId = _fixedBlogId,
                Status = 0,
                Note = "Từ chối",
            };

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(_fixedUserId);
            _blogRepoMock
                .Setup(r => r.ChangeStatusBlogAsync(_fixedBlogId, 0, "Từ chối", _fixedUserId))
                .ReturnsAsync(false);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(500));
            Assert.That(response.Message, Is.EqualTo(Message.UPDATE_STATUS_BLOG_FAIL));
            _blogRepoMock.Verify(
                r => r.ChangeStatusBlogAsync(_fixedBlogId, 0, "Từ chối", _fixedUserId),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_WhenUserIdIsNull_ReturnsUnauthorized()
        {
            // Arrange
            var request = new ChangeStatusBlogRequest
            {
                BlogId = _fixedBlogId,
                Status = 1,
                Note = "Note test",
            };

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns((string)null);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(401));
            Assert.That(response.Message, Is.EqualTo(Message.UNAUTHORIZED));
            _blogRepoMock.Verify(
                r =>
                    r.ChangeStatusBlogAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<int>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    ),
                Times.Never
            );
        }

        [Test]
        public async Task Handle_WhenUserIdIsEmpty_ReturnsUnauthorized()
        {
            // Arrange
            var request = new ChangeStatusBlogRequest
            {
                BlogId = _fixedBlogId,
                Status = 2,
                Note = "Note test",
            };

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(string.Empty);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(401));
            Assert.That(response.Message, Is.EqualTo(Message.UNAUTHORIZED));
            _blogRepoMock.Verify(
                r =>
                    r.ChangeStatusBlogAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<int>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    ),
                Times.Never
            );
        }

        [Test]
        public async Task Handle_WhenNoteIsNull_StillProcessesCorrectly()
        {
            // Arrange
            var request = new ChangeStatusBlogRequest
            {
                BlogId = _fixedBlogId,
                Status = 1,
                Note = null,
            };

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(_fixedUserId);
            _blogRepoMock
                .Setup(r => r.ChangeStatusBlogAsync(_fixedBlogId, 1, null, _fixedUserId))
                .ReturnsAsync(true);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo(Message.UPDATE_STATUS_BLOG_SUCCESS));
            _blogRepoMock.Verify(
                r => r.ChangeStatusBlogAsync(_fixedBlogId, 1, null, _fixedUserId),
                Times.Once
            );
        }
    }
}
