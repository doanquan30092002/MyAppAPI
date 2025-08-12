using Moq;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.DeleteBlog.Tests
{
    [TestFixture()]
    public class DeleteBlogHandlerTests
    {
        private Mock<IBlogRepository> _blogRepoMock;
        private DeleteBlogHandler _handler;
        private readonly Guid _fixedBlogId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        [SetUp]
        public void Setup()
        {
            _blogRepoMock = new Mock<IBlogRepository>();
            _handler = new DeleteBlogHandler(_blogRepoMock.Object);
        }

        [Test]
        public async Task Handle_DeleteBlogSuccess_ReturnsTrue()
        {
            // Arrange
            _blogRepoMock.Setup(r => r.DeleteBlogAsync(_fixedBlogId)).ReturnsAsync(true);

            var request = new DeleteBlogRequest { BlogId = _fixedBlogId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            _blogRepoMock.Verify(r => r.DeleteBlogAsync(_fixedBlogId), Times.Once);
        }

        [Test]
        public async Task Handle_DeleteBlogFail_ReturnsFalse()
        {
            // Arrange
            _blogRepoMock.Setup(r => r.DeleteBlogAsync(_fixedBlogId)).ReturnsAsync(false);

            var request = new DeleteBlogRequest { BlogId = _fixedBlogId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _blogRepoMock.Verify(r => r.DeleteBlogAsync(_fixedBlogId), Times.Once);
        }
    }
}
