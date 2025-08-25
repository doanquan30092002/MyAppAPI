using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.Interfaces.IGetListBidders;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;

namespace MyApp.Application.CQRS.GetListBidders.Queries
{
    public class GetListBiddersHandler
        : IRequestHandler<GetListBiddersRequest, GetListBiddersResponse>
    {
        private readonly IGetListBiddersRepository _getListBidders;

        public GetListBiddersHandler(IGetListBiddersRepository getListBiddersRepository)
        {
            _getListBidders =
                getListBiddersRepository
                ?? throw new ArgumentNullException(nameof(getListBiddersRepository));
        }

        public Task<GetListBiddersResponse> Handle(
            GetListBiddersRequest request,
            CancellationToken cancellationToken
        )
        {
            return _getListBidders.GetListBidders(request);
        }
    }
}
