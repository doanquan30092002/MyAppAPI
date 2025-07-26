using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.UpdateBlog
{
    public class UpdateBlogHandler : IRequestHandler<UpdateBlogRequest, UpdateBlogResponse>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadFile _uploadFileService;

        public UpdateBlogHandler(
            IBlogRepository blogRepository,
            IHttpContextAccessor httpContextAccessor,
            IUploadFile uploadFileService
        )
        {
            _blogRepository = blogRepository;
            _httpContextAccessor = httpContextAccessor;
            _uploadFileService = uploadFileService;
        }

        public async Task<UpdateBlogResponse> Handle(
            UpdateBlogRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
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
                userIdStr
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
