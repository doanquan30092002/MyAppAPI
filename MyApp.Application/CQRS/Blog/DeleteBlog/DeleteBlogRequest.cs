using MediatR;

namespace MyApp.Application.CQRS.Blog.DeleteBlog
{
    public class DeleteBlogRequest : IRequest<bool>
    {
        public Guid BlogId { get; set; }
    }
}
