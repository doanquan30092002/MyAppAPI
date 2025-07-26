namespace MyApp.Application.CQRS.Blog.GetBlogDetail
{
    public class GetBlogDetailResponse
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
