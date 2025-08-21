using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.ChangeStatusBlog
{
    public class ChangeStatusBlogHandler
        : IRequestHandler<ChangeStatusBlogRequest, ChangeStatusBlogResponse>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ICurrentUserService _currentUserService;

        public ChangeStatusBlogHandler(
            IBlogRepository blogRepository,
            ICurrentUserService currentUserService
        )
        {
            this._blogRepository = blogRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ChangeStatusBlogResponse> Handle(
            ChangeStatusBlogRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return new ChangeStatusBlogResponse { Code = 401, Message = Message.UNAUTHORIZED };
            }
            bool isChangeSuccess = await _blogRepository.ChangeStatusBlogAsync(
                request.BlogId,
                request.Status,
                request.Note,
                userId
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
