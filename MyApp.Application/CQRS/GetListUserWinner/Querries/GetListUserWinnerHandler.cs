using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.GetListUserWinner.Queries;
using MyApp.Application.CQRS.GetListUserWinner.Querries;
using MyApp.Application.Interfaces.IListUserWinnerRepository;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.GetListUserWinner.Queries
{
    public class GetListUserWinnerHandler
        : IRequestHandler<GetListUserWinnerRequest, GetListUserWinnerResponse>
    {
        private readonly IListUserWinnerRepository _getListUserWinnerRepository;

        public GetListUserWinnerHandler(IListUserWinnerRepository getListUserWinnerRepository)
        {
            _getListUserWinnerRepository = getListUserWinnerRepository;
        }

        public async Task<GetListUserWinnerResponse> Handle(
            GetListUserWinnerRequest request,
            CancellationToken cancellationToken
        )
        {
            var response = await _getListUserWinnerRepository.GetListUserWinnerAsync(request);

            return response;
        }
    }
}
