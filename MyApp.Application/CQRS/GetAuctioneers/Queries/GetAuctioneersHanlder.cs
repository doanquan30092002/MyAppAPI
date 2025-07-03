using MediatR;
using MyApp.Application.Interfaces.GetAuctioneers;

namespace MyApp.Application.CQRS.GetAuctioneers.Queries
{
    public class GetAuctioneersHanlder
        : IRequestHandler<GetAuctioneersRequest, List<GetAuctioneersResponse>>
    {
        private readonly IGetAuctioneersRepository _getAuctioneersRepository;

        public GetAuctioneersHanlder(IGetAuctioneersRepository getAuctioneersRepository)
        {
            _getAuctioneersRepository = getAuctioneersRepository;
        }

        public Task<List<GetAuctioneersResponse>> Handle(
            GetAuctioneersRequest request,
            CancellationToken cancellationToken
        )
        {
            var auctioneers = _getAuctioneersRepository.GetAuctioneersAsync();
            return auctioneers;
        }
    }
}
