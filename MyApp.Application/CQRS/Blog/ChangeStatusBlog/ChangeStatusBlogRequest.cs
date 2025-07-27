using MediatR;

namespace MyApp.Application.CQRS.Blog.ChangeStatusBlog
{
    public class ChangeStatusBlogRequest : IRequest<ChangeStatusBlogResponse>
    {
        public Guid BlogId { get; set; }
        public int Status { get; set; }
        public string? Note { get; set; }
    }
}
