using MediatR;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.GetBlogDetail
{
    public class GetBlogDetailHandler : IRequestHandler<GetBlogDetailRequest, GetBlogDetailResponse>
    {
        private readonly IBlogRepository _blogRepository;

        public GetBlogDetailHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<GetBlogDetailResponse?> Handle(
            GetBlogDetailRequest request,
            CancellationToken cancellationToken
        )
        {
            GetBlogDetailResponse? blogDetail = await _blogRepository.GetBlogDetailAsync(
                request.BlogId
            );
            return blogDetail;
        }
    }
}
