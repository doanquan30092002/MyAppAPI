using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.CreateBlog.Tests
{
    [TestFixture()]
    public class CreateBlogHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IUploadFile> _uploadFileMock;
        private CreateBlogHandler _handler;
        private readonly string _fixedUserId = "22222222-2222-2222-2222-222222222222";

        [SetUp]
        public void Setup()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _uploadFileMock = new Mock<IUploadFile>();

            _handler = new CreateBlogHandler(
                _blogRepoMock.Object,
                _currentUserServiceMock.Object,
                _uploadFileMock.Object
            );

            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(_fixedUserId);
        }

        [Test]
        public async Task Handle_WithThumbnail_CreateSuccess_ReturnsSuccess()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);

            _uploadFileMock
                .Setup(u => u.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("http://image.url/test.png");

            _blogRepoMock
                .Setup(r =>
                    r.CreateBlogAsync(
                        "Tiêu đề",
                        "Nội dung",
                        "http://image.url/test.png",
                        _fixedUserId
                    )
                )
                .ReturnsAsync(true);

            var request = new CreateBlogRequest
            {
                Title = "Tiêu đề",
                Content = "Nội dung",
                Thumbnail = fileMock.Object,
            };

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo(Message.CREATE_BLOG_SUCCESS));
        }

        [Test]
        public async Task Handle_NoThumbnail_CreateSuccess_ReturnsSuccess()
        {
            _blogRepoMock
                .Setup(r => r.CreateBlogAsync("Tiêu đề", "Nội dung", "", _fixedUserId))
                .ReturnsAsync(true);

            var request = new CreateBlogRequest
            {
                Title = "Tiêu đề",
                Content = "Nội dung",
                Thumbnail = null,
            };

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo(Message.CREATE_BLOG_SUCCESS));
            _uploadFileMock.Verify(u => u.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Handle_WithThumbnail_CreateFail_ReturnsFail()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);

            _uploadFileMock
                .Setup(u => u.UploadAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("http://image.url/test.png");

            _blogRepoMock
                .Setup(r =>
                    r.CreateBlogAsync(
                        "Tiêu đề",
                        "Nội dung",
                        "http://image.url/test.png",
                        _fixedUserId
                    )
                )
                .ReturnsAsync(false);

            var request = new CreateBlogRequest
            {
                Title = "Tiêu đề",
                Content = "Nội dung",
                Thumbnail = fileMock.Object,
            };

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.That(response.Code, Is.EqualTo(500));
            Assert.That(response.Message, Is.EqualTo(Message.CREATE_BLOG_FAIL));
        }

        [Test]
        public async Task Handle_ThumbnailLengthZero_CreateSuccess_ReturnsSuccess()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            _blogRepoMock
                .Setup(r => r.CreateBlogAsync("Tiêu đề", "Nội dung", "", _fixedUserId))
                .ReturnsAsync(true);

            var request = new CreateBlogRequest
            {
                Title = "Tiêu đề",
                Content = "Nội dung",
                Thumbnail = fileMock.Object,
            };

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo(Message.CREATE_BLOG_SUCCESS));
            _uploadFileMock.Verify(u => u.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Handle_UserIdNull_ReturnsUnauthorized()
        {
            _currentUserServiceMock.Setup(s => s.GetUserId()).Returns((string)null);

            var request = new CreateBlogRequest { Title = "Tiêu đề", Content = "Nội dung" };

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.That(response.Code, Is.EqualTo(401));
            Assert.That(response.Message, Is.EqualTo(Message.UNAUTHORIZED));
            _blogRepoMock.Verify(
                r =>
                    r.CreateBlogAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    ),
                Times.Never
            );
        }
    }
}
