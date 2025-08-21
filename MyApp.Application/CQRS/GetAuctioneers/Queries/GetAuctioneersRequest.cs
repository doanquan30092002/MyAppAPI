using MediatR;

namespace MyApp.Application.CQRS.GetAuctioneers.Queries
{
    public class GetAuctioneersRequest : IRequest<List<GetAuctioneersResponse>> { }
}
