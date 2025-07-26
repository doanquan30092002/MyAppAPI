using MediatR;

namespace MyApp.Application.CQRS.Blog.GetBlogDetail
{
    public class GetBlogDetailRequest : IRequest<GetBlogDetailResponse>
    {
        public Guid BlogId { get; set; }
    }
}
