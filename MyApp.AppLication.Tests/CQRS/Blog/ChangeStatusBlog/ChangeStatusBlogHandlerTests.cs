using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.ChangeStatusBlog.Tests
{
    [TestFixture()]
    public class ChangeStatusBlogHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private ChangeStatusBlogHandler _handler;

        // Dùng Guid cố định cho predict dễ dàng
        private readonly Guid _fixedBlogId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private readonly string _fixedUserId = "22222222-2222-2222-2222-222222222222";

        [SetUp]
        public void Setup()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _handler = new ChangeStatusBlogHandler(
                _blogRepoMock.Object,
                _httpContextAccessorMock.Object
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

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, _fixedUserId) })
            );
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

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

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, _fixedUserId) })
            );
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

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
    }
}
