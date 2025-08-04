using MyApp.Application.CQRS.Blog.GetBlogDetail;
using MyApp.Application.CQRS.Blog.GetListBlog;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.Blog
{
    public interface IBlogRepository
    {
        Task<bool> ChangeStatusBlogAsync(Guid blogId, int status, string? note, string? userIdStr);
        Task<bool> CreateBlogAsync(
            string title,
            string content,
            string thumbnailUrl,
            string? userIdStr
        );
        Task<bool> DeleteBlogAsync(Guid blogId);
        Task<GetBlogDetailResponse> GetBlogDetailAsync(Guid blogId);
        Task<List<BlogsResponse>> GetListBlogAsync(
            int pageNumber,
            int pageSize,
            string? searchTitle,
            int? status = null,
            Guid? userId = null
        );
        Task<int> GetTotalCountAsync(string? searchTitle, int? status = null, Guid? userId = null);
        Task<bool> UpdateBlogAsync(
            Guid blogId,
            string? title,
            string? content,
            string? thumbnailUrl,
            string? userIdStr
        );
    }
}
