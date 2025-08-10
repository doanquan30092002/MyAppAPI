using MediatR;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.CQRS.Blog.UpdateBlog
{
    public class UpdateBlogRequest : IRequest<UpdateBlogResponse>
    {
        public Guid BlogId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
