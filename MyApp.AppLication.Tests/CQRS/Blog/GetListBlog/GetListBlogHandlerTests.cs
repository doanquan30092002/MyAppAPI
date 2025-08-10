using Moq;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.GetListBlog.Tests
{
    [TestFixture()]
    public class GetListBlogHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private GetListBlogHandler _handler;
        private readonly Guid _fixedBlogId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        [SetUp]
        public void SetUp()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _handler = new GetListBlogHandler(_blogRepoMock.Object);
        }

        [Test]
        public async Task Handle_BlogsFound_ReturnsFullList()
        {
            // Arrange
            var blogs = new List<BlogsResponse>
            {
                new BlogsResponse
                {
                    BlogId = _fixedBlogId,
                    Title = "Bài viết 1",
                    Content = "Nội dung 1",
                    ThumbnailUrl = "http://example.com/thumb1.jpg",
                    CreatedAt = new DateTime(2025, 1, 1),
                    CreatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UpdatedAt = new DateTime(2025, 2, 2),
                    UpdatedBy = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Status = 1,
                    Note = "Ghi chú 1",
                },
                new BlogsResponse
                {
                    BlogId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Title = "Bài viết 2",
                    Content = "Nội dung 2",
                    ThumbnailUrl = "http://example.com/thumb2.jpg",
                    CreatedAt = new DateTime(2025, 3, 3),
                    CreatedBy = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    UpdatedAt = new DateTime(2025, 4, 4),
                    UpdatedBy = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Status = 2,
                    Note = "Ghi chú 2",
                },
            };

            var request = new GetListBlogRequest
            {
                PageNumber = 1,
                PageSize = 10,
                SearchTitle = "test",
                Status = 1,
                UserId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            };

            _blogRepoMock
                .Setup(r =>
                    r.GetListBlogAsync(
                        request.PageNumber,
                        request.PageSize,
                        request.SearchTitle,
                        request.Status,
                        request.UserId
                    )
                )
                .ReturnsAsync(blogs);

            _blogRepoMock
                .Setup(r =>
                    r.GetTotalCountAsync(request.SearchTitle, request.Status, request.UserId)
                )
                .ReturnsAsync(100);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageNumber, Is.EqualTo(request.PageNumber));
            Assert.That(result.PageSize, Is.EqualTo(request.PageSize));
            Assert.That(result.TotalCount, Is.EqualTo(100));
            Assert.That(result.Blogs.Count, Is.EqualTo(2));

            Assert.That(result.Blogs[0].BlogId, Is.EqualTo(blogs[0].BlogId));
            Assert.That(result.Blogs[0].Title, Is.EqualTo(blogs[0].Title));
            Assert.That(result.Blogs[0].Content, Is.EqualTo(blogs[0].Content));
            Assert.That(result.Blogs[0].ThumbnailUrl, Is.EqualTo(blogs[0].ThumbnailUrl));
            Assert.That(result.Blogs[0].CreatedAt, Is.EqualTo(blogs[0].CreatedAt));
            Assert.That(result.Blogs[0].CreatedBy, Is.EqualTo(blogs[0].CreatedBy));
            Assert.That(result.Blogs[0].UpdatedAt, Is.EqualTo(blogs[0].UpdatedAt));
            Assert.That(result.Blogs[0].UpdatedBy, Is.EqualTo(blogs[0].UpdatedBy));
            Assert.That(result.Blogs[0].Status, Is.EqualTo(blogs[0].Status));
            Assert.That(result.Blogs[0].Note, Is.EqualTo(blogs[0].Note));

            _blogRepoMock.Verify(
                r =>
                    r.GetListBlogAsync(
                        request.PageNumber,
                        request.PageSize,
                        request.SearchTitle,
                        request.Status,
                        request.UserId
                    ),
                Times.Once
            );

            _blogRepoMock.Verify(
                r => r.GetTotalCountAsync(request.SearchTitle, request.Status, request.UserId),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_NoBlogsFound_ReturnsEmptyList()
        {
            // Arrange
            var request = new GetListBlogRequest { PageNumber = 1, PageSize = 10 };

            _blogRepoMock
                .Setup(r =>
                    r.GetListBlogAsync(
                        request.PageNumber,
                        request.PageSize,
                        request.SearchTitle,
                        request.Status,
                        request.UserId
                    )
                )
                .ReturnsAsync(new List<BlogsResponse>());

            _blogRepoMock
                .Setup(r =>
                    r.GetTotalCountAsync(request.SearchTitle, request.Status, request.UserId)
                )
                .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Blogs, Is.Empty);
            Assert.That(result.TotalCount, Is.EqualTo(0));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(10));

            _blogRepoMock.Verify(
                r =>
                    r.GetListBlogAsync(
                        request.PageNumber,
                        request.PageSize,
                        request.SearchTitle,
                        request.Status,
                        request.UserId
                    ),
                Times.Once
            );

            _blogRepoMock.Verify(
                r => r.GetTotalCountAsync(request.SearchTitle, request.Status, request.UserId),
                Times.Once
            );
        }
    }
}
