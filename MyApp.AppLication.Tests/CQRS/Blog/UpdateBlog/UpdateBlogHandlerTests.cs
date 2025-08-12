using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.UpdateBlog.Tests
{
    [TestFixture()]
    public class UpdateBlogHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IUploadFile> _uploadFileMock;
        private UpdateBlogHandler _handler;

        private static readonly Guid FixedBlogId = new("11111111-1111-1111-1111-111111111111");
        private const string FixedUserId = "22222222-2222-2222-2222-222222222222";

        [SetUp]
        public void Setup()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _uploadFileMock = new Mock<IUploadFile>();

            _handler = new UpdateBlogHandler(
                _blogRepoMock.Object,
                _currentUserServiceMock.Object,
                _uploadFileMock.Object
            );

            _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(FixedUserId);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenUpdateBlogWithoutThumbnail()
        {
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

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo(Message.UPDATE_BLOG_SUCCESS));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.BlogId, Is.EqualTo(FixedBlogId));
            _uploadFileMock.Verify(u => u.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenUpdateBlogWithThumbnail()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);

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

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo(Message.UPDATE_BLOG_SUCCESS));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.BlogId, Is.EqualTo(FixedBlogId));
            _uploadFileMock.Verify(u => u.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenUpdateBlogFails()
        {
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

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(500));
            Assert.That(result.Message, Is.EqualTo(Message.UPDATE_BLOG_FAIL));
            Assert.That(result.Data, Is.Null);
        }

        [Test]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsNull()
        {
            _currentUserServiceMock.Setup(x => x.GetUserId()).Returns((string)null);

            var request = new UpdateBlogRequest
            {
                BlogId = FixedBlogId,
                Title = "Tiêu đề",
                Content = "Nội dung",
            };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(401));
            Assert.That(result.Message, Is.EqualTo(Message.UNAUTHORIZED));
            Assert.That(result.Data, Is.Null);

            _blogRepoMock.Verify(
                x =>
                    x.UpdateBlogAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    ),
                Times.Never
            );
        }

        [Test]
        public async Task Handle_ShouldNotUpload_WhenThumbnailLengthIsZero()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0); // length = 0

            var request = new UpdateBlogRequest
            {
                BlogId = FixedBlogId,
                Title = "Tiêu đề",
                Content = "Nội dung",
                Thumbnail = fileMock.Object,
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

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo(Message.UPDATE_BLOG_SUCCESS));
            Assert.That(result.Data.BlogId, Is.EqualTo(FixedBlogId));
            _uploadFileMock.Verify(u => u.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }
    }
}
