using MediatR;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.GetListBlog
{
    public class GetListBlogHandler : IRequestHandler<GetListBlogRequest, GetListBlogResponse>
    {
        private readonly IBlogRepository _blogRepository;

        public GetListBlogHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<GetListBlogResponse> Handle(
            GetListBlogRequest request,
            CancellationToken cancellationToken
        )
        {
            var blogs = await _blogRepository.GetListBlogAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTitle,
                request.Status,
                request.UserId
            );
            return new GetListBlogResponse
            {
                Blogs = blogs,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = await _blogRepository.GetTotalCountAsync(
                    request.SearchTitle,
                    request.Status,
                    request.UserId
                ),
            };
        }
    }
}
