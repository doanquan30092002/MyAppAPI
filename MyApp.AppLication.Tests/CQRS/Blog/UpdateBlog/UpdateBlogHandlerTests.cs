using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.UpdateBlog.Tests
{
    [TestFixture()]
    public class UpdateBlogHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private Mock<IHttpContextAccessor> _httpContextMock;
        private Mock<IUploadFile> _uploadFileMock;
        private UpdateBlogHandler _handler;

        private static readonly Guid FixedBlogId = new Guid("11111111-1111-1111-1111-111111111111");
        private const string FixedUserId = "22222222-2222-2222-2222-222222222222";

        [SetUp]
        public void Setup()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _httpContextMock = new Mock<IHttpContextAccessor>();
            _uploadFileMock = new Mock<IUploadFile>();

            _handler = new UpdateBlogHandler(
                _blogRepoMock.Object,
                _httpContextMock.Object,
                _uploadFileMock.Object
            );

            // Mock user in HttpContext
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, FixedUserId) })
            );
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextMock.Setup(x => x.HttpContext).Returns(httpContext);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenUpdateBlogWithoutThumbnail()
        {
            // Arrange
            var request = new UpdateBlogRequest
            {
                BlogId = FixedBlogId,
                Title = "Tiêu đề mới",
                Content = "Nội dung mới",
                Thumbnail = null,
            };

            _blogRepoMock
                .Setup(x =>
                    x.UpdateBlogAsync(
                        FixedBlogId,
                        request.Title,
                        request.Content,
                        null,
                        FixedUserId
                    )
                )
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo(Message.UPDATE_BLOG_SUCCESS));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.BlogId, Is.EqualTo(FixedBlogId));
            _uploadFileMock.Verify(u => u.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenUpdateBlogWithThumbnail()
        {
            // Arrange: mock IFormFile
            var fileMock = new Mock<IFormFile>();
            var content = "fake image content";
            var fileName = "thumbnail.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.ContentType).Returns("image/jpeg");

            var request = new UpdateBlogRequest
            {
                BlogId = FixedBlogId,
                Title = "Tiêu đề mới",
                Content = "Nội dung mới",
                Thumbnail = fileMock.Object,
            };

            var uploadedUrl = "https://example.com/thumbnail.jpg";

            _uploadFileMock.Setup(u => u.UploadAsync(request.Thumbnail)).ReturnsAsync(uploadedUrl);

            _blogRepoMock
                .Setup(x =>
                    x.UpdateBlogAsync(
                        FixedBlogId,
                        request.Title,
                        request.Content,
                        uploadedUrl,
                        FixedUserId
                    )
                )
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo(Message.UPDATE_BLOG_SUCCESS));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.BlogId, Is.EqualTo(FixedBlogId));
            _uploadFileMock.Verify(u => u.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
            _blogRepoMock.Verify(
                x =>
                    x.UpdateBlogAsync(
                        FixedBlogId,
                        request.Title,
                        request.Content,
                        uploadedUrl,
                        FixedUserId
                    ),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenUpdateBlogFails()
        {
            // Arrange
            var request = new UpdateBlogRequest
            {
                BlogId = FixedBlogId,
                Title = "Tiêu đề lỗi",
                Content = "Nội dung lỗi",
                Thumbnail = null,
            };

            _blogRepoMock
                .Setup(x =>
                    x.UpdateBlogAsync(
                        FixedBlogId,
                        request.Title,
                        request.Content,
                        null,
                        FixedUserId
                    )
                )
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.Code, Is.EqualTo(500));
            Assert.That(result.Message, Is.EqualTo(Message.UPDATE_BLOG_FAIL));
            Assert.That(result.Data, Is.Null);
        }
    }
}
