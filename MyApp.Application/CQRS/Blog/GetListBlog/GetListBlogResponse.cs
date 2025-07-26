using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.Blog.GetListBlog
{
    public class GetListBlogResponse
    {
        public List<BlogsResponse> Blogs { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class BlogsResponse
    {
        public Guid BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public int Status { get; set; }
    }
}
