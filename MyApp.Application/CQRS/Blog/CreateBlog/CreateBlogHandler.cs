using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.Blog;

namespace MyApp.Application.CQRS.Blog.CreateBlog
{
    public class CreateBlogHandler : IRequestHandler<CreateBlogRequest, CreateBlogResponse>
    {
        private readonly IBlogRepository _createBlogRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUploadFile _uploadFileService;

        public CreateBlogHandler(
            IBlogRepository createBlogRepository,
            ICurrentUserService currentUserService,
            IUploadFile uploadFileService
        )
        {
            _createBlogRepository = createBlogRepository;
            _currentUserService = currentUserService;
            _uploadFileService = uploadFileService;
        }

        public async Task<CreateBlogResponse> Handle(
            CreateBlogRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return new CreateBlogResponse { Code = 401, Message = Message.UNAUTHORIZED };
            }
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
                userId
            );
            if (!createBlogStatus)
            {
                return new CreateBlogResponse { Code = 500, Message = Message.CREATE_BLOG_FAIL };
            }
            return new CreateBlogResponse { Code = 200, Message = Message.CREATE_BLOG_SUCCESS };
        }
    }
}
