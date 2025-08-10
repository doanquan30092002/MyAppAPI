using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.CreateBlog.Tests
{
    [TestFixture()]
    public class CreateBlogHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IUploadFile> _uploadFileMock;
        private CreateBlogHandler _handler;
        private readonly string _fixedUserId = "22222222-2222-2222-2222-222222222222";

        [SetUp]
        public void Setup()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _uploadFileMock = new Mock<IUploadFile>();

            _handler = new CreateBlogHandler(
                _blogRepoMock.Object,
                _httpContextAccessorMock.Object,
                _uploadFileMock.Object
            );

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, _fixedUserId) })
            );
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);
        }

        [Test]
        public async Task Handle_WithThumbnail_CreateSuccess_ReturnsSuccess()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "fake image content";
            var fileName = "test.png";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);

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

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo(Message.CREATE_BLOG_SUCCESS));
        }

        [Test]
        public async Task Handle_NoThumbnail_CreateSuccess_ReturnsSuccess()
        {
            // Arrange
            _blogRepoMock
                .Setup(r => r.CreateBlogAsync("Tiêu đề", "Nội dung", "", _fixedUserId))
                .ReturnsAsync(true);

            var request = new CreateBlogRequest
            {
                Title = "Tiêu đề",
                Content = "Nội dung",
                Thumbnail = null,
            };

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo(Message.CREATE_BLOG_SUCCESS));
            _uploadFileMock.Verify(u => u.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Handle_WithThumbnail_CreateFail_ReturnsFail()
        {
            // Arrange
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

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(response.Code, Is.EqualTo(500));
            Assert.That(response.Message, Is.EqualTo(Message.CREATE_BLOG_FAIL));
        }
    }
}
