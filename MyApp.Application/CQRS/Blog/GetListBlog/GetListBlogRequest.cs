using MediatR;

namespace MyApp.Application.CQRS.Blog.GetListBlog
{
    public class GetListBlogRequest : IRequest<GetListBlogResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SearchTitle { get; set; }
        public int? Status { get; set; }
        public Guid? UserId { get; set; }
    }
}
