using MediatR;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.DeleteBlog
{
    public class DeleteBlogHandler : IRequestHandler<DeleteBlogRequest, bool>
    {
        private readonly IBlogRepository _blogRepository;

        public DeleteBlogHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<bool> Handle(
            DeleteBlogRequest request,
            CancellationToken cancellationToken
        )
        {
            bool isDeleted = await _blogRepository.DeleteBlogAsync(request.BlogId);

            return isDeleted;
        }
    }
}
