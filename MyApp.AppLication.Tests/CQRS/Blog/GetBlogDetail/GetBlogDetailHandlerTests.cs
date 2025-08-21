using Moq;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.GetBlogDetail.Tests
{
    [TestFixture()]
    public class GetBlogDetailHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private GetBlogDetailHandler _handler;
        private readonly Guid _fixedBlogId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        [SetUp]
        public void Setup()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _handler = new GetBlogDetailHandler(_blogRepoMock.Object);
        }

        [Test]
        public async Task Handle_BlogFound_ReturnsBlogDetail()
        {
            // Arrange
            var expectedBlog = new GetBlogDetailResponse
            {
                BlogId = _fixedBlogId,
                Title = "Bài viết test",
                Content = "Nội dung test",
                ThumbnailUrl = "http://example.com/thumb.jpg",
                CreatedAt = new DateTime(2025, 1, 1),
                CreatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                UpdatedAt = new DateTime(2025, 2, 2),
                UpdatedBy = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Status = 1,
                Note = "Ghi chú test",
            };

            _blogRepoMock.Setup(r => r.GetBlogDetailAsync(_fixedBlogId)).ReturnsAsync(expectedBlog);

            var request = new GetBlogDetailRequest { BlogId = _fixedBlogId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BlogId, Is.EqualTo(expectedBlog.BlogId));
            Assert.That(result.Title, Is.EqualTo(expectedBlog.Title));
            Assert.That(result.Content, Is.EqualTo(expectedBlog.Content));
            Assert.That(result.ThumbnailUrl, Is.EqualTo(expectedBlog.ThumbnailUrl));
            Assert.That(result.CreatedAt, Is.EqualTo(expectedBlog.CreatedAt));
            Assert.That(result.CreatedBy, Is.EqualTo(expectedBlog.CreatedBy));
            Assert.That(result.UpdatedAt, Is.EqualTo(expectedBlog.UpdatedAt));
            Assert.That(result.UpdatedBy, Is.EqualTo(expectedBlog.UpdatedBy));
            Assert.That(result.Status, Is.EqualTo(expectedBlog.Status));
            Assert.That(result.Note, Is.EqualTo(expectedBlog.Note));

            _blogRepoMock.Verify(r => r.GetBlogDetailAsync(_fixedBlogId), Times.Once);
        }

        [Test]
        public async Task Handle_BlogNotFound_ReturnsNull()
        {
            // Arrange
            _blogRepoMock
                .Setup(r => r.GetBlogDetailAsync(_fixedBlogId))
                .ReturnsAsync((GetBlogDetailResponse?)null);

            var request = new GetBlogDetailRequest { BlogId = _fixedBlogId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
            _blogRepoMock.Verify(r => r.GetBlogDetailAsync(_fixedBlogId), Times.Once);
        }
    }
}
