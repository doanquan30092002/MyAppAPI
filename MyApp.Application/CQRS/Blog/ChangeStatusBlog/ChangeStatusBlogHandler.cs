using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.ChangeStatusBlog
{
    public class ChangeStatusBlogHandler
        : IRequestHandler<ChangeStatusBlogRequest, ChangeStatusBlogResponse>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangeStatusBlogHandler(
            IBlogRepository blogRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this._blogRepository = blogRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChangeStatusBlogResponse> Handle(
            ChangeStatusBlogRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            bool isChangeSuccess = await _blogRepository.ChangeStatusBlogAsync(
                request.BlogId,
                request.Status,
                request.Note,
                userIdStr
            );
            if (!isChangeSuccess)
            {
                return new ChangeStatusBlogResponse()
                {
                    Code = 500,
                    Message = Message.UPDATE_STATUS_BLOG_FAIL,
                };
            }
            return new ChangeStatusBlogResponse()
            {
                Code = 200,
                Message = Message.UPDATE_STATUS_BLOG_SUCCESS,
            };
        }
    }
}
