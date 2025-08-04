using MediatR;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.CQRS.Blog.CreateBlog
{
    public class CreateBlogRequest : IRequest<CreateBlogResponse>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public IFormFile Thumbnail { get; set; }
    }
}
