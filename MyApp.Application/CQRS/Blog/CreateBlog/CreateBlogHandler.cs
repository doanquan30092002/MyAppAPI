using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.CreateBlog
{
    public class CreateBlogHandler : IRequestHandler<CreateBlogRequest, CreateBlogResponse>
    {
        private readonly IBlogRepository _createBlogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadFile _uploadFileService;

        public CreateBlogHandler(
            IBlogRepository createBlogRepository,
            IHttpContextAccessor httpContextAccessor,
            IUploadFile uploadFileService
        )
        {
            _createBlogRepository = createBlogRepository;
            _httpContextAccessor = httpContextAccessor;
            _uploadFileService = uploadFileService;
        }

        public async Task<CreateBlogResponse> Handle(
            CreateBlogRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            // upload file
            string thumbnailUrl = "";
            if (request.Thumbnail != null && request.Thumbnail.Length > 0)
            {
                thumbnailUrl = await _uploadFileService.UploadAsync(request.Thumbnail);
            }
            // create blog
            bool createBlogStatus = await _createBlogRepository.CreateBlogAsync(
                request.Title,
                request.Content,
                thumbnailUrl,
                userIdStr
            );
            if (!createBlogStatus)
            {
                return new CreateBlogResponse { Code = 500, Message = Message.CREATE_BLOG_FAIL };
            }
            return new CreateBlogResponse { Code = 200, Message = Message.CREATE_BLOG_SUCCESS };
        }
    }
}
