using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.UpdateBlog
{
    public class UpdateBlogHandler : IRequestHandler<UpdateBlogRequest, UpdateBlogResponse>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUploadFile _uploadFileService;

        public UpdateBlogHandler(
            IBlogRepository blogRepository,
            ICurrentUserService currentUserService,
            IUploadFile uploadFileService
        )
        {
            _blogRepository = blogRepository;
            _currentUserService = currentUserService;
            _uploadFileService = uploadFileService;
        }

        public async Task<UpdateBlogResponse> Handle(
            UpdateBlogRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return new UpdateBlogResponse
                {
                    Code = 401,
                    Message = Message.UNAUTHORIZED,
                    Data = null,
                };
            }
            // upload file
            string thumbnailUrl = null;
            if (request.Thumbnail != null && request.Thumbnail.Length > 0)
            {
                thumbnailUrl = await _uploadFileService.UploadAsync(request.Thumbnail);
            }
            bool updateBlogStatus = await _blogRepository.UpdateBlogAsync(
                request.BlogId,
                request.Title,
                request.Content,
                thumbnailUrl,
                userId
            );
            if (!updateBlogStatus)
            {
                return new UpdateBlogResponse
                {
                    Code = 500,
                    Message = Message.UPDATE_BLOG_FAIL,
                    Data = null,
                };
            }
            return new UpdateBlogResponse
            {
                Code = 200,
                Message = Message.UPDATE_BLOG_SUCCESS,
                Data = new UpdateBlogResponseDTO { BlogId = request.BlogId },
            };
        }
    }
}
