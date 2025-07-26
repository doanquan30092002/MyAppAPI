namespace MyApp.Application.CQRS.Blog.UpdateBlog
{
    public class UpdateBlogResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public UpdateBlogResponseDTO? Data { get; set; }
    }

    public class UpdateBlogResponseDTO
    {
        public Guid BlogId { get; set; }
    }
}
